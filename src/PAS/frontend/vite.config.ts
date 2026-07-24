import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

const serviceUrl = (name: string, fallback: string) =>
  process.env[`services__${name}__https__0`] ??
  process.env[`services__${name}__http__0`] ??
  fallback

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    strictPort: true,
    proxy: {
      '/api/assets': {
        target: serviceUrl('pas-asset-api', 'http://localhost:5198'),
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api\/assets/, ''),
      },
      '/api/calculations': {
        target: serviceUrl('pas-calculation-api', 'http://localhost:5113'),
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api\/calculations/, ''),
      },
    },
  },
})
