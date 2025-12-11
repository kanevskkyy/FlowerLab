import "./Cabinet.css";

export default function Cabinet() {
  return (
    <div className="cabinet-page">
      {/* ===== HEADER ===== */}
      <header className="cabinet-header">
        <div className="header-left">
          <button className="burger-btn" aria-label="Menu">
            <span />
            <span />
            <span />
          </button>
          <span className="lang-switch">UA/ENG</span>
        </div>

        <div className="header-center">[LOGO]</div>

        <div className="header-right">
          <span className="currency-switch">UAH/USD</span>

          <button className="icon-btn" aria-label="Bag">
            <div className="icon-bag" />
          </button>

          <div className="header-user">
            <div className="icon-user" />
            <div className="header-user-text">
              <span className="header-user-caption">Signed in</span>
            </div>
          </div>
        </div>
      </header>

      {/* ===== BODY ===== */}
      <div className="cabinet-body">
        {/* ===== SIDEBAR ===== */}
        <aside className="cabinet-sidebar">
          <nav className="sidebar-nav">
            <button className="sidebar-item sidebar-item-active">
              <div className="sidebar-icon circle-icon">
                <div className="circle-inner" />
              </div>
              <span>Personal information</span>
            </button>

            <button className="sidebar-item">
              <div className="sidebar-icon rect-icon" />
              <span>My orders</span>
            </button>

            <button className="sidebar-item">
              <div className="sidebar-icon home-icon">
                <div className="home-roof" />
                <div className="home-body" />
              </div>
              <span>Saved addresses</span>
            </button>
          </nav>

          <button className="sidebar-signout">
            <div className="signout-icon">
              <span className="signout-arrow" />
              <span className="signout-box" />
            </div>
            <span>Sign out</span>
          </button>
        </aside>

        {/* ===== MAIN CONTENT ===== */}
        <main className="cabinet-content">
          <h1 className="content-title">Personal information</h1>

          {/* First/Last name */}
          <div className="form-row">
            <div className="form-field">
              <label>First Name</label>
              <input type="text" placeholder="Name" />
            </div>

            <div className="form-field">
              <label>Last Name</label>
              <input type="text" placeholder="Name" />
            </div>
          </div>

          {/* Phone */}
          <div className="form-row single">
            <div className="form-field">
              <label>Phone Number</label>
              <input type="text" placeholder="+38 050 159 19 12" />
            </div>
          </div>

          {/* Account information title */}
          <div className="section-title">Account information</div>

          {/* Email + Password */}
          <div className="account-row">
            <div className="account-card">
              <div className="account-card-left">
                <div className="card-icon mail-icon">
                  <div className="mail-envelope" />
                </div>
                <span>youremail@gmail.com</span>
              </div>
              <button className="card-action-btn">Change</button>
            </div>

            <div className="account-card">
              <div className="account-card-left">
                <div className="card-icon lock-icon">
                  <div className="lock-body" />
                  <div className="lock-loop" />
                </div>
                <span>Password</span>
              </div>
              <button className="card-action-btn">Change</button>
            </div>
          </div>

          {/* Delete account */}
          <div className="account-row full">
            <div className="account-card delete-card">
              <div className="account-card-left">
                <div className="card-icon trash-icon">
                  <div className="trash-body" />
                  <div className="trash-lid" />
                </div>
                <span>Delete account</span>
              </div>
            </div>
          </div>

          {/* Save changes button */}
          <div className="save-wrapper">
            <button className="save-btn">Save changes</button>
          </div>
        </main>
      </div>
    </div>
  );
}
