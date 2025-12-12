// src/pages/SideMenu/SideMenu.jsx
import './SideMenu.css';

const TEXT = {
  en: {
    home: 'HOME PAGE',
    about: 'ABOUT US',
    catalog: 'CATALOG',
    categories: ['Bouquets', 'Baskets', 'Boxes', 'Gifts', 'Ballons'],
  },
  ua: {
    home: 'ГОЛОВНА',
    about: 'ПРО НАС',
    catalog: 'КАТАЛОГ',
    categories: ['Букети', 'Корзини', 'Коробки', 'Подарунки', 'Повітряні кульки'],
  },
};

export default function SideMenu({ language = 'en', onClose }) {
  const t = TEXT[language];

  return (
    <div className="side-menu">
      <button className="side-menu-close" onClick={onClose}>
        ×
      </button>

      <nav className="side-menu-nav">
        <button className="side-menu-item">{t.home}</button>
        <button className="side-menu-item">{t.about}</button>

        <div className="side-menu-block">
          <div className="side-menu-item catalog-row">
            <span>{t.catalog}</span>
            <span className="catalog-chevron">⌄</span>
          </div>

          <ul className="side-menu-sublist">
            {t.categories.map((c) => (
              <li key={c} className="side-menu-subitem">
                {c}
              </li>
            ))}
          </ul>
        </div>
      </nav>
    </div>
  );
}
