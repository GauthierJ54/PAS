import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { initializeKeycloak } from './auth/keycloak'
import './api/configureClients'
import './index.css'
import App from './App.tsx'

async function bootstrap() {
  await initializeKeycloak()

  const rootElement = document.getElementById('root')

  if (!rootElement) {
    throw new Error("L'élément root est introuvable.")
  }

  createRoot(rootElement).render(
    <StrictMode>
      <App />
    </StrictMode>,
  )
}

bootstrap().catch((error: unknown) => {
  console.error('Initialisation Keycloak impossible.', error)

  const rootElement = document.getElementById('root')

  if (rootElement) {
    rootElement.innerHTML = `
      <main style="font-family: sans-serif; padding: 40px">
        <h1>Connexion impossible</h1>
        <p>Vérifie que Keycloak est démarré sur le port 8080.</p>
      </main>
    `
  }
})