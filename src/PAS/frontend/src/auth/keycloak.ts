import Keycloak from 'keycloak-js'

export const keycloak = new Keycloak({
  url: import.meta.env.VITE_KEYCLOAK_URL,
  realm: import.meta.env.VITE_KEYCLOAK_REALM,
  clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
})

export async function initializeKeycloak(): Promise<void> {
  const authenticated = await keycloak.init({
    onLoad: 'login-required',
    pkceMethod: 'S256',
    checkLoginIframe: false,
  })

  if (!authenticated) {
    await keycloak.login()
  }
}

export async function getAccessToken(): Promise<string> {
  if (!keycloak.authenticated) {
    throw new Error('Utilisateur non authentifié.')
  }

  await keycloak.updateToken(30)

  if (!keycloak.token) {
    throw new Error('Aucun access token disponible.')
  }

  return keycloak.token
}

export async function logout(): Promise<void> {
  await keycloak.logout({
    redirectUri: window.location.origin,
    logoutMethod: 'POST',
  })
}
