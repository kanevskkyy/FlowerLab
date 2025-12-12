import './Dashboard.css';

export default function Dashboard() {
  return (
    <div className="dashboard">
      <h2 className="dashboard-title">Analytics</h2>

      <div className="dashboard-cards">
        <div className="dashboard-card">
          <span>Total customers</span>
          <strong>1000</strong>
        </div>
        <div className="dashboard-card">
          <span>Total orders</span>
          <strong>1000</strong>
        </div>
        <div className="dashboard-card">
          <span>Total bouquets sold</span>
          <strong>1000</strong>
        </div>
      </div>

      <h3 className="dashboard-subtitle">Overview</h3>
      <div className="dashboard-overview" />

      <div className="dashboard-bottom">
        <div>
          <span>This month revenue:</span>
          <strong>1000</strong>
        </div>
        <div>
          <span>Sales this month:</span>
          <strong>1000</strong>
        </div>
      </div>
    </div>
  );
}
