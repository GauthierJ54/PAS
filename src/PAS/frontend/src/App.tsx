import { useCallback, useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { ApiError as AssetApiError } from './api/generated/asset'
import type { FundDto } from './api/generated/asset'
import { FundsService as AssetApi } from './api/generated/asset'
import type { DailyPerformanceDto } from './api/generated/calculation'
import { FundPerformancesService as CalculationApi } from './api/generated/calculation'
import { keycloak, logout } from './auth/keycloak'
import './App.css'

const fundStatus: Record<number, string> = {
  0: 'Brouillon',
  1: 'Actif',
  2: 'Inactif',
}

const today = new Date().toISOString().slice(0, 10)

function errorMessage(error: unknown) {
  if (error instanceof AssetApiError) {
    const body = error.body as { detail?: string; title?: string; error?: string } | undefined
    return body?.detail ?? body?.error ?? body?.title ?? error.message
  }
  return error instanceof Error ? error.message : 'Une erreur inattendue est survenue.'
}

function App() {
  const [funds, setFunds] = useState<FundDto[]>([])
  const [selectedFund, setSelectedFund] = useState<FundDto | null>(null)
  const [loading, setLoading] = useState(false)
  const [notice, setNotice] = useState('')
  const [error, setError] = useState('')

  const [createForm, setCreateForm] = useState({ name: '', isin: '', currency: 'EUR' })
  const [navForm, setNavForm] = useState({ value: '', date: today })
  const [performanceForm, setPerformanceForm] = useState({ date: today })
  const [performance, setPerformance] = useState<DailyPerformanceDto | null>(null)

  const canWrite = keycloak.hasRealmRole('funds.write')
  const canDelete = keycloak.hasRealmRole('funds.delete')

  const run = async (action: () => Promise<void>, successMessage?: string) => {
    setLoading(true)
    setError('')
    setNotice('')
    try {
      await action()
      if (successMessage) setNotice(successMessage)
    } catch (caught) {
      setError(errorMessage(caught))
    } finally {
      setLoading(false)
    }
  }

  const loadFunds = useCallback(async () => {
    if (!canWrite && !canDelete) {
      const result = await AssetApi.getFundsFilter(undefined, undefined, undefined, 'Active')
      setFunds(result)
    } else {
      const result = await AssetApi.getAllFunds()
      setFunds(result)
    }
  }, [])

  useEffect(() => {
    let active = true
    if (!canWrite && !canDelete) {
      AssetApi.getFundsFilter(undefined, undefined, undefined, 'Active')
        .then((result) => {
          if (active) setFunds(result)
        })
        .catch((caught) => {
          if (active) setError(errorMessage(caught))
        })
    } else {
      AssetApi.getAllFunds()
        .then((result) => {
          if (active) setFunds(result)
        })
        .catch((caught) => {
          if (active) setError(errorMessage(caught))
        })
    }

    return () => {
      active = false
    }
  }, [])

  const showFund = async (id: string) => {
    await run(async () => {
      const fund = await AssetApi.getFundById(id)
      setSelectedFund(fund)
      setNavForm((current) => ({ ...current, fundId: fund.id }))
      setPerformanceForm((current) => ({ ...current, fundId: fund.id }))
    })
  }

  const createFund = async (event: FormEvent) => {
    event.preventDefault()
    await run(async () => {
      const created = await AssetApi.createFund(createForm)
      setCreateForm({ name: '', isin: '', currency: 'EUR' })
      await loadFunds()
      await showFund(created.id)
    }, 'Fond créé avec succès.')
  }

  const addNav = async (event: FormEvent) => {
    if (!selectedFund) {
      setError('Sélectionnez un fond avant d’ajouter une valeur liquidative.')
      return
    }
    event.preventDefault()
    await run(async () => {
      await AssetApi.addFundNav(selectedFund.id, {
        value: Number(navForm.value),
        date: `${navForm.date}T00:00:00.000Z`,
      })
      await loadFunds()
      await showFund(selectedFund.id)
    }, 'Valeur liquidative ajoutée.')
  }

  const changeFundStatus = async (id: string, status: string) => {
    if (!window.confirm('Modifier le statut de ce fond ?')) return
    await run(async () => {
      await AssetApi.updateFundStatus(id, status)
      if (selectedFund?.id === id) setSelectedFund(null)
      await loadFunds()
    }, 'Statut du fond modifié.')
  }

  const deleteFund = async (id: string) => {
    if (!window.confirm('Supprimer logiquement ce fond ?')) return
    await run(async () => {
      await AssetApi.softDeleteFund(id)
      if (selectedFund?.id === id) setSelectedFund(null)
      await loadFunds()
    }, ' supprimé.')
  }

  const deleteNav = async (fundId: string, date: string) => {
    if (!window.confirm(`Supprimer la VL du ${date} ?`)) return
    await run(async () => {
      await AssetApi.softDeleteFundNav(fundId, `${date}T00:00:00.000Z`)
      await loadFunds()
      await showFund(fundId)
    }, 'Valeur liquidative supprimée.')
  }

  const getPerformance = async (event: FormEvent) => {
    if (!selectedFund) {
      setError('Sélectionnez un fond avant de calculer la performance.')
      return
    }
    event.preventDefault()
    await run(async () => {
      const result = await CalculationApi.getDailyPerformance(
        selectedFund?.id,
        performanceForm.date,
      )
      setPerformance(result)
    })
  }

  return (
    <main className="app-shell">
      <header className="topbar">
        <div>
          <span className="eyebrow">PAS · Policy Administration System</span>
          <h1>Funds console</h1>
          <p>Une interface pour les API Asset et Calculation.</p>
        </div>
        <div>
          <button className="secondary" onClick={() => run(loadFunds)} disabled={loading}>
            Actualiser
          </button>
          <span style={{ margin: '0 1rem' }}>
            <strong>{keycloak.tokenParsed?.preferred_username}</strong> ({keycloak.tokenParsed?.email})
          </span>

          <button
            type="button"
            className="secondary"
            onClick={() => void logout()}
          >
            Se déconnecter
          </button>
        </div>
      </header>

      {(notice || error) && (
        <div className={`message ${error ? 'message-error' : 'message-success'}`}>
          {error || notice}
        </div>
      )}

      <section className="stats">
        <article><strong>{funds.length}</strong><span>fonds disponibles</span></article>
        <article><strong>{funds.reduce((total, fund) => total + fund.navs.length, 0)}</strong><span>valeurs nettes d'inventaire</span></article>
        <article><strong>{selectedFund ? selectedFund.currency : '—'}</strong><span>devise sélectionnée</span></article>
      </section>

      <div className="grid">
        <section className="panel panel-wide">
          <div className="panel-title">
            <div><span className="section-number">01</span><h2>Fonds</h2></div>
            <span className="muted">Asset API</span>
          </div>

          <div className="table-wrap">
            <table>
              <thead><tr><th>Nom</th><th>ISIN</th><th>Devise</th><th>Statut</th><th>VNI</th><th></th></tr></thead>
              <tbody>
                {funds.map((fund) => (
                  <tr key={fund.id} onClick={() => showFund(fund.id)} className={selectedFund?.id === fund.id ? 'selected-row' : ''}>
                    <td>{fund.name}</td>
                    <td className="mono">{fund.isin}</td>
                    <td>{fund.currency}</td>
                    <td><span className="status">{fundStatus[fund.status] ?? fund.status}</span></td>
                    <td>{fund.navs.length}</td>
                    <td><button className="danger-link" disabled={!canDelete} onClick={() => deleteFund(fund.id)}>Supprimer</button></td>
                  </tr>
                ))}
                {!funds.length && <tr><td colSpan={6} className="empty">Aucun fond trouvé.</td></tr>}
              </tbody>
            </table>
          </div>
        </section>

        <section className="panel">
          <div className="panel-title"><div><span className="section-number">02</span><h2>Créer un fond</h2></div></div>
          <form className="stack" onSubmit={createFund}>
            <label>Nom<input value={createForm.name} onChange={(e) => setCreateForm({ ...createForm, name: e.target.value })} required /></label>
            <label>ISIN<input value={createForm.isin} onChange={(e) => setCreateForm({ ...createForm, isin: e.target.value })} required /></label>
            <label>Devise<input value={createForm.currency} onChange={(e) => setCreateForm({ ...createForm, currency: e.target.value.toUpperCase() })} maxLength={3} required /></label>
            <button disabled={loading || !canWrite}>Créer le fond</button>
          </form>
        </section>

        <section className="panel">
          <div className="panel-title"><div><span className="section-number">03</span><h2>Ajouter une VNI</h2></div></div>
          <form className="stack" onSubmit={addNav}>
            <label>Valeur<input type="number" min="0" step="0.0001" value={navForm.value} onChange={(e) => setNavForm({ ...navForm, value: e.target.value })} required /></label>
            <label>Date<input type="date" value={navForm.date} onChange={(e) => setNavForm({ ...navForm, date: e.target.value })} required /></label>
            <button disabled={loading || !canWrite}>Ajouter la VNI</button>
          </form>
        </section>

        <section className="panel panel-wide">
          <div className="panel-title">
            <div><span className="section-number">04</span><h2>Détail du fond</h2></div>
            <span className="muted">GetFundById</span>
          </div>
          {selectedFund ? (
            <>
              <div className="fund-heading">
                <div><h3>{selectedFund.name}</h3><span className="mono">{selectedFund.id}</span></div>
                <div className="fund-meta">
                  <select value={selectedFund.status} onChange={(e) => changeFundStatus(selectedFund.id, e.target.value)} disabled={!canWrite}>
                    <option value="0">Brouillon</option>
                    <option value="1">Actif</option>
                    <option value="2">Inactif</option>
                  </select>
                  <span>{selectedFund.isin}</span>
                  <span>{selectedFund.currency}</span>
                </div>
              </div>
              <div className="nav-list">
                {selectedFund.navs.map((nav) => (
                  <div className="nav-item" key={nav.date}>
                    <span>{nav.date}</span>
                    <strong>{nav.value.toLocaleString('fr-FR')} {selectedFund.currency}</strong>
                    <button className="danger-link" disabled={!canDelete} onClick={() => deleteNav(selectedFund.id, nav.date)}>Supprimer</button>
                  </div>
                ))}
                {!selectedFund.navs.length && <p className="empty">Ce fond ne possède aucune VNI.</p>}
              </div>
            </>
          ) : <p className="empty">Sélectionnez un fond dans la liste.</p>}
        </section>

        <section className="panel panel-wide performance-panel">
          <div className="panel-title">
            <div><span className="section-number">05</span><h2>Performance journalière</h2></div>
            <span className="muted">Calculation API</span>
          </div>
          <form className="performance-form" onSubmit={getPerformance}>
            <label>Date<input type="date" value={performanceForm.date} onChange={(e) => setPerformanceForm({ ...performanceForm, date: e.target.value })} required /></label>
            <button disabled={loading}>Calculer</button>
          </form>
          {performance && (
            <div className="performance-result">
              <div><span>VNI précédente</span><strong>{performance.previousValue.toLocaleString('fr-FR')}</strong></div>
              <div><span>VNI courante</span><strong>{performance.currentValue.toLocaleString('fr-FR')}</strong></div>
              <div className={Number(performance.rate) >= 0 ? 'positive' : 'negative'}><span>Performance</span><strong>{(Number(performance.rate) * 100).toFixed(2)} %</strong></div>
            </div>
          )}
        </section>
      </div>

      {loading && <div className="loading-bar" aria-label="Chargement" />}
    </main>
  )
}

export default App



