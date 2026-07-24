import { mkdir, writeFile } from 'node:fs/promises';
import { spawnSync } from 'node:child_process';
import { fileURLToPath } from 'node:url';
import path from 'node:path';

const frontendDirectory = path.resolve(
  path.dirname(fileURLToPath(import.meta.url)),
  '..',
);

const specifications = {
  asset: {
    url:
      process.env.PAS_ASSET_OPENAPI_URL ??
      'http://localhost:5198/openapi/v1.json',
    output: 'src/api/generated/asset',
  },
  calculation: {
    url:
      process.env.PAS_CALCULATION_OPENAPI_URL ??
      'http://localhost:5113/openapi/v1.json',
    output: 'src/api/generated/calculation',
  },
};

const requestedApi = process.argv[2];

if (requestedApi && !(requestedApi in specifications)) {
  console.error(
    `API inconnue "${requestedApi}". Valeurs possibles : ${Object.keys(specifications).join(', ')}.`,
  );
  process.exit(1);
}

const apiNames = requestedApi ? [requestedApi] : Object.keys(specifications);
const downloadDirectory = path.join(frontendDirectory, '.openapi');
const openApiCli = path.join(
  frontendDirectory,
  'node_modules',
  'openapi-typescript-codegen',
  'bin',
  'index.js',
);

await mkdir(downloadDirectory, { recursive: true });

for (const apiName of apiNames) {
  const specification = specifications[apiName];
  console.log(`Téléchargement de la spécification ${apiName}: ${specification.url}`);

  const response = await fetch(specification.url);

  if (!response.ok) {
    throw new Error(
      `Impossible de télécharger ${specification.url}: HTTP ${response.status} ${response.statusText}`,
    );
  }

  const document = await response.text();
  JSON.parse(document);

  const inputFile = path.join(downloadDirectory, `${apiName}.json`);
  await writeFile(inputFile, document, 'utf8');

  console.log(`Génération du client ${apiName}...`);

  const result = spawnSync(
    process.execPath,
    [
      openApiCli,
      '--input',
      inputFile,
      '--output',
      path.join(frontendDirectory, specification.output),
      '--client',
      'axios',
    ],
    {
      cwd: frontendDirectory,
      stdio: 'inherit',
    },
  );

  if (result.error) {
    throw result.error;
  }

  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
}

console.log('Clients API générés avec succès.');

