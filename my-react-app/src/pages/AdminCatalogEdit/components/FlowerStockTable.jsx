import React from "react";
import editIcon from "../../../assets/icons/edit.svg";
import trashIcon from "../../../assets/icons/trash.svg";

const FlowerStockTable = ({
  flowers,
  inputs,
  onInputChange,
  onAdd,
  onRemove,
  editingFlower,
  setEditingFlower,
  onUpdateFlower,
}) => {
  return (
    <div className="ace-card full-width">
      <h3 className="ace-card-title">Flower Types & Stock</h3>

      <table className="ace-table">
        <thead>
          <tr>
            <th>Name</th>
            <th style={{ width: "120px" }}>Quantity</th>
            <th style={{ width: "140px", textAlign: "right" }}>Actions</th>
          </tr>
        </thead>
        <tbody>
          {/* Add Row */}
          <tr className="ace-table-add-row">
            <td>
              <input
                type="text"
                placeholder="New Flower Name..."
                value={inputs.flowerTypesName}
                onChange={(e) =>
                  onInputChange("flowerTypesName", e.target.value)
                }
                onKeyDown={(e) => e.key === "Enter" && onAdd("flowerTypes")}
              />
            </td>
            <td>
              <input
                type="number"
                placeholder="0"
                value={inputs.flowerTypesQuantity}
                onChange={(e) =>
                  onInputChange("flowerTypesQuantity", e.target.value)
                }
                onKeyDown={(e) => e.key === "Enter" && onAdd("flowerTypes")}
              />
            </td>
            <td style={{ textAlign: "right" }}>
              <button
                className="ace-add-btn small"
                onClick={() => onAdd("flowerTypes")}>
                Add
              </button>
            </td>
          </tr>

          {/* List Rows */}
          {flowers.map((item) => {
            const isEditing = editingFlower?.id === item.id;

            if (isEditing) {
              return (
                <tr key={item.id} className="editing-row">
                  <td>
                    <input
                      type="text"
                      value={editingFlower.name}
                      onChange={(e) =>
                        setEditingFlower({
                          ...editingFlower,
                          name: e.target.value,
                        })
                      }
                    />
                  </td>
                  <td>
                    <input
                      type="number"
                      value={editingFlower.quantity}
                      onChange={(e) =>
                        setEditingFlower({
                          ...editingFlower,
                          quantity: parseInt(e.target.value) || 0,
                        })
                      }
                    />
                  </td>
                  <td style={{ textAlign: "right" }}>
                    <button
                      className="ace-icon-btn save"
                      onClick={onUpdateFlower}>
                      Save
                    </button>
                    <button
                      className="ace-icon-btn cancel"
                      onClick={() => setEditingFlower(null)}>
                      ✕
                    </button>
                  </td>
                </tr>
              );
            }

            return (
              <tr key={item.id}>
                <td>{item.name}</td>
                <td>{item.quantity}</td>
                <td style={{ textAlign: "right" }}>
                  <button
                    className="ace-icon-btn edit"
                    onClick={() => setEditingFlower(item)}>
                    <img src={editIcon} alt="edit" />
                  </button>
                  <button
                    className="ace-icon-btn delete"
                    onClick={() => onRemove("flowerTypes", item)}>
                    <img src={trashIcon} alt="trash" />
                  </button>
                </td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
};

export default FlowerStockTable;
