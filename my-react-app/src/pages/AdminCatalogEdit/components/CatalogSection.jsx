import React from "react";

const CatalogSection = ({
  title,
  items,
  inputValue,
  onInputChange,
  onAdd,
  onRemove,
  onKeyDown,
}) => {
  return (
    <div className="ace-card">
      <h3 className="ace-card-title">{title}</h3>

      {/* Tags List */}
      <div className="ace-tags-list">
        {items.map((item) => (
          <div key={item.id} className="ace-tag">
            <span>{item.name}</span>
            <button
              type="button"
              className="ace-tag-remove"
              onClick={() => onRemove(item)}>
              ✕
            </button>
          </div>
        ))}
        {items.length === 0 && <div className="ace-empty">No items yet</div>}
      </div>

      {/* Add Row */}
      <div className="ace-add-row">
        <input
          type="text"
          placeholder={`Add new ${title.toLowerCase()}...`}
          value={inputValue}
          onChange={(e) => onInputChange(e.target.value)}
          onKeyDown={onKeyDown}
        />
        <button type="button" className="ace-add-btn" onClick={onAdd}>
          Add
        </button>
      </div>
    </div>
  );
};

export default CatalogSection;
