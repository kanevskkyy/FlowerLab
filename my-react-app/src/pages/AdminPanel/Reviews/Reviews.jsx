import './Reviews.css';

const mockReviews = [
  {
    id: 1,
    name: 'Name Surname',
    rating: 5,
    text: 'i really like the bouquet and recommend this store',
  },
  {
    id: 2,
    name: 'Name Surname',
    rating: 4,
    text: 'i really like the bouquet and recommend this store',
  },
];

export default function Reviews() {
  return (
    <div className="reviews-page">
      <h2 className="reviews-title">Pending reviews</h2>

      <div className="reviews-list">
        {mockReviews.map((r) => (
          <ReviewCard key={r.id} review={r} />
        ))}
      </div>
    </div>
  );
}

function ReviewCard({ review }) {
  const stars = Array.from({ length: 5 });

  return (
    <div className="review-card">
      {/* Ліва частина: аватар + текст */}
      <div className="review-left">
        <div className="review-avatar" />

        <div className="review-content">
          <div className="review-name">{review.name}</div>

          <div className="review-stars">
            {stars.map((_, i) => (
              <span
                key={i}
                className={i < review.rating ? 'star filled' : 'star'}
              >
                ★
              </span>
            ))}
          </div>

          <div className="review-text">{review.text}</div>
        </div>
      </div>

      {/* Права частина: Post / Delete */}
      <div className="review-actions-panel">
        <div className="review-actions-top">
          <span>Post</span>
          <span>Delete</span>
        </div>

        <div className="review-actions-bottom">
          <button className="review-action-btn" title="Post">
            ✓
          </button>
          <button className="review-action-btn" title="Delete">
            🗑
          </button>
        </div>
      </div>
    </div>
  );
}
