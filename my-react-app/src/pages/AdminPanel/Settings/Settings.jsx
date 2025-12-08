import './Settings.css';

export default function Settings() {
  return (
    <div className="settings-page">
      <div className="settings-header-row">
        <h2 className="settings-title">Store management</h2>

        <button className="settings-edit-btn">
          Edit ✏️
        </button>
      </div>

      {/* Shop location information */}
      <section className="settings-section">
        <h3 className="settings-section-title">Shop location information:</h3>
        <ul className="settings-list">
          <li>м. Чернівці, вул. Герцена 2а</li>
          <li>м. Чернівці, вул. Васіле Александрі, 1</li>
        </ul>
      </section>

      {/* Working hours */}
      <section className="settings-section">
        <div className="settings-working-header">Working hours</div>

        <div className="settings-radio-group">
          <label className="settings-radio-item">
            <span className="radio-circle checked">✓</span>
            <span>09:00 -- 20:00</span>
          </label>

          <label className="settings-radio-item">
            <span className="radio-circle" />
            <span>10:00 -- 19:00</span>
          </label>
        </div>
      </section>

      {/* Social media links */}
      <section className="settings-section">
        <h3 className="settings-section-title">Social media links:</h3>
        <ul className="settings-list links">
          <li>https://www.instagram.com/flowerlab_vlada</li>
          <li>https://www.instagram.com/flowerlab_vlada</li>
        </ul>
      </section>

      {/* Contact details */}
      <section className="settings-section">
        <h3 className="settings-section-title">Contact details:</h3>
        <ul className="settings-list">
          <li>+38 050 159 19 12</li>
          <li>+38 050 159 18 13</li>
        </ul>
      </section>
    </div>
  );
}
