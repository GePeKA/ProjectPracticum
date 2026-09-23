import { timeZoneChoices, useLocale, type Language } from '../locale'

export function SettingsPage() {
  const { language, timeZone, setLanguage, setTimeZone, t, formatWhen } = useLocale()
  const zones = timeZoneChoices.includes(timeZone) ? timeZoneChoices : [timeZone, ...timeZoneChoices]

  return (
    <>
      <h1>{t('settings')}</h1>
      <p className="lead">{t('settingsLead')}</p>
      <form onSubmit={event => event.preventDefault()}>
        <label>
          {t('language')}
          <select value={language} onChange={event => setLanguage(event.target.value as Language)}>
            <option value="ru">Русский</option>
            <option value="en">English</option>
          </select>
        </label>
        <label>
          {t('timeZone')}
          <select value={timeZone} onChange={event => setTimeZone(event.target.value)}>
            {zones.map(zone => <option key={zone} value={zone}>{zone}</option>)}
          </select>
        </label>
        <p className="quiet">{t('timeSample')}: {formatWhen(new Date().toISOString())}</p>
      </form>
    </>
  )
}
