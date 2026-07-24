import { OpenAPI as AssetOpenAPI } from './generated/asset'
import { OpenAPI as CalculationOpenAPI } from './generated/calculation'
import { getAccessToken } from '../auth/keycloak'

AssetOpenAPI.BASE = '/api/assets'
AssetOpenAPI.TOKEN = getAccessToken

CalculationOpenAPI.BASE = '/api/calculations'
CalculationOpenAPI.TOKEN = getAccessToken