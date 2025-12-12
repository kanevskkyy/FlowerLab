import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import PopupMenu from "../popupMenu/PopupMenu";
import Header from "../../components/Header/Header";
import Footer from "../../components/Footer/Footer";
import { useCart } from "../../context/CartContext";
import "./OrderPlacementRegistered.css";
import ShoppingBagIcon from "../../assets/images/ShoppingBagIcon.svg";
// Import images for gifts
import gift1 from "../../assets/images/gift1.jpg";
import gift2 from "../../assets/images/gift2.jpg";
import gift3 from "../../assets/images/gift3.png";

const OrderPlacement = () => {
  const navigate = useNavigate();
  const { cartItems, removeItem } = useCart();
  const [menuOpen, setMenuOpen] = useState(false);
  const [isRegistered, setIsRegistered] = useState(true);
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    phone: "",
    receiverType: "self",
    deliveryType: "delivery",
    message: "",
    noCall: false
  });

  // Адреси для доставки
  const [deliveryAddresses, setDeliveryAddresses] = useState([
    { id: 1, text: "м. Чернівці, вул. Головна 446, кв. 12", isActive: true }
  ]);
  
  // Адреси магазинів для самовивозу
  const shopAddresses = [
    { id: 1, text: "м. Чернівці, вул Герцена 2а", isActive: true },
    { id: 2, text: "вул Васіле Александрі, 1", isActive: false }
  ];

  const [selectedShopAddress, setSelectedShopAddress] = useState(1);
  const [isAddingAddress, setIsAddingAddress] = useState(false);
  const [newAddress, setNewAddress] = useState("");

  const [selectedGifts, setSelectedGifts] = useState([]);
  const [isCardAdded, setIsCardAdded] = useState(false);

  const gifts = [
    { id: 1, name: "Gift", price: 1000, img: gift1 },
    { id: 2, name: "Star Baloon", price: 1000, img: gift2 },
    { id: 3, name: "Gift", price: 1000, img: gift3 }
  ];

  // Calculate totals
  const subtotal = cartItems.reduce((sum, item) => {
    const price = typeof item.price === 'string' 
      ? parseFloat(item.price.replace(/[^\d]/g, '')) 
      : item.price;
    const qty = item.qty || 1;
    return sum + (price * qty);
  }, 0);
  
  const delivery = formData.deliveryType === "delivery" ? 100 : 0;
  const discount = subtotal * 0.1; // 10%
  const giftsTotal = selectedGifts.reduce((sum, id) => {
    const gift = gifts.find(g => g.id === id);
    return sum + (gift?.price || 0);
  }, 0);
  const cardFee = isCardAdded ? 50 : 0;
  const total = subtotal + delivery - discount + giftsTotal + cardFee;

  const handleInputChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value
    }));
  };

  const toggleGift = (giftId) => {
    setSelectedGifts(prev => 
      prev.includes(giftId) 
        ? prev.filter(id => id !== giftId)
        : [...prev, giftId]
    );
  };

  const toggleCard = () => {
    setIsCardAdded(prev => !prev);
  };

  const handleAddAddress = () => {
    if (newAddress.trim()) {
      const newId = Math.max(...deliveryAddresses.map(a => a.id), 0) + 1;
      setDeliveryAddresses(prev => [
        ...prev.map(a => ({ ...a, isActive: false })),
        { id: newId, text: newAddress, isActive: true }
      ]);
      setNewAddress("");
      setIsAddingAddress(false);
    }
  };

  const selectDeliveryAddress = (id) => {
    setDeliveryAddresses(prev => 
      prev.map(a => ({ ...a, isActive: a.id === id }))
    );
  };

  const selectShopAddress = (id) => {
    setSelectedShopAddress(id);
  };

  // ✅ ВИПРАВЛЕНО: Додано перенаправлення на CheckOut
  const handleConfirmOrder = () => {
    const orderData = {
      formData,
      cartItems,
      selectedGifts,
      isCardAdded,
      total,
      selectedAddress: formData.deliveryType === "delivery" 
        ? deliveryAddresses.find(a => a.isActive)?.text
        : shopAddresses.find(a => a.id === selectedShopAddress)?.text
    };
    console.log("Order confirmed:", orderData);
    
    // ✅ Перенаправлення на сторінку оплати з передачею даних
    navigate("/checkout", { state: { orderData } });
  };

  return (
    <div className="page-wrapper order-page">
      <Header onMenuOpen={() => setMenuOpen(true)} />
      <PopupMenu isOpen={menuOpen} onClose={() => setMenuOpen(false)} />
      <main className="order-placement">
        <h1 className="order-title">ORDER PLACEMENT</h1>

        <button className="back-btn" onClick={() => navigate(-1)}>
          <span>←</span> Back to shopping
        </button>

        <div className="order-content">
          {/* LEFT SIDE - Form */}
          <div className="order-form-section">
            
            {/* Contact Information */}
            <section className="form-section">
              <h2>Your contact information</h2>

              <div className="registration-tabs">
                <button 
                  className={isRegistered ? "tab active" : "tab"}
                  onClick={() => setIsRegistered(true)}
                >
                  I am registered
                </button>
                <button 
                  className={!isRegistered ? "tab active" : "tab"}
                  onClick={() => setIsRegistered(false)}
                >
                  Buy without registering
                </button>
              </div>

              <div className="form-group">
                <label>First Name</label>
                <input
                  type="text"
                  name="firstName"
                  placeholder="First Name"
                  value={formData.firstName}
                  onChange={handleInputChange}
                />
              </div>

              <div className="form-group">
                <label>Last Name</label>
                <input
                  type="text"
                  name="lastName"
                  placeholder="Last Name"
                  value={formData.lastName}
                  onChange={handleInputChange}
                />
              </div>

              <div className="form-group">
                <label>Phone</label>
                <input
                  type="tel"
                  name="phone"
                  placeholder="+38 066 001 02 03"
                  value={formData.phone}
                  onChange={handleInputChange}
                />
              </div>

              <div className="receiver-section">
                <label>Receiver:</label>
                <div className="radio-group">
                  <label className="radio-label">
                    <input
                      type="radio"
                      name="receiverType"
                      value="self"
                      checked={formData.receiverType === "self"}
                      onChange={handleInputChange}
                    />
                    <span>I am the receiver</span>
                  </label>
                  <label className="radio-label">
                    <input
                      type="radio"
                      name="receiverType"
                      value="other"
                      checked={formData.receiverType === "other"}
                      onChange={handleInputChange}
                    />
                    <span>The receiver is other person</span>
                  </label>
                </div>
              </div>
            </section>

            {/* Delivery */}
            <section className="form-section">
              <h2>Delivery</h2>

              <div className="delivery-tabs">
                <button 
                  className={formData.deliveryType === "pickup" ? "delivery-tab active" : "delivery-tab"}
                  onClick={() => setFormData(prev => ({ ...prev, deliveryType: "pickup" }))}
                >
                  Pickup at the shop
                </button>
                <button 
                  className={formData.deliveryType === "delivery" ? "delivery-tab active" : "delivery-tab"}
                  onClick={() => setFormData(prev => ({ ...prev, deliveryType: "delivery" }))}
                >
                  Delivery
                </button>
              </div>

              {/* DELIVERY - показуємо адреси доставки */}
              {formData.deliveryType === "delivery" && (
                <>
                  <div className="addresses-list">
                    {deliveryAddresses.map(addr => (
                      <div 
                        key={addr.id} 
                        className={`address-item ${addr.isActive ? 'active' : ''}`}
                        onClick={() => selectDeliveryAddress(addr.id)}
                      >
                        <input
                          type="radio"
                          name="selectedAddress"
                          checked={addr.isActive}
                          readOnly
                        />
                        <span>{addr.text}</span>
                      </div>
                    ))}
                  </div>

                  {isAddingAddress ? (
                    <div className="new-address-form">
                      <input
                        type="text"
                        placeholder="Enter new address"
                        value={newAddress}
                        onChange={(e) => setNewAddress(e.target.value)}
                        className="new-address-input"
                      />
                      <div className="new-address-buttons">
                        <button onClick={handleAddAddress} className="save-address-btn">
                          Save
                        </button>
                        <button 
                          onClick={() => {
                            setIsAddingAddress(false);
                            setNewAddress("");
                          }} 
                          className="cancel-address-btn"
                        >
                          Cancel
                        </button>
                      </div>
                    </div>
                  ) : (
                    <button 
                      className="add-address-btn"
                      onClick={() => setIsAddingAddress(true)}
                    >
                      + Add a new address
                    </button>
                  )}
                </>
              )}

              {/* PICKUP - показуємо адреси магазинів */}
              {formData.deliveryType === "pickup" && (
                <div className="addresses-list">
                  {shopAddresses.map(addr => (
                    <div 
                      key={addr.id} 
                      className={`address-item ${selectedShopAddress === addr.id ? 'active' : ''}`}
                      onClick={() => selectShopAddress(addr.id)}
                    >
                      <input
                        type="radio"
                        name="selectedShopAddress"
                        checked={selectedShopAddress === addr.id}
                        readOnly
                      />
                      <span>{addr.text}</span>
                    </div>
                  ))}
                </div>
              )}
            </section>

            {/* Gifts */}
            <section className="form-section">
              <h2>Gifts</h2>

              <div className="gifts-grid">
                {gifts.map(gift => (
                  <div key={gift.id} className="gift-card">
                    <div className="gift-image">
                      <img src={gift.img} alt={gift.name} />
                    </div>
                    <div className="gift-bottom">
                      <div className="gift-info">
                        <p className="gift-name">{gift.name}</p>
                        <p className="gift-price">{gift.price} ₴</p>
                      </div>
                      <button 
                        className={`add-gift-btn ${selectedGifts.includes(gift.id) ? 'active' : ''}`}
                        onClick={() => toggleGift(gift.id)}
                      >
                        +
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          </div>

          {/* RIGHT SIDE - Order Summary */}
          <div className="order-summary">
            <h2>YOUR ORDER:</h2>

            <div className="order-items">
              {cartItems.map(item => (
                <div key={item.id} className="order-item">
                  <img src={item.img} alt={item.title} />
                  <div className="item-details">
                    <p className="item-name">{item.title}</p>
                    <p className="item-price">
                      {typeof item.price === 'string' ? item.price : `${item.price} ₴`}
                    </p>
                    <p className="item-quantity">{item.qty || 1} pc</p>
                  </div>
                  <button 
                    className="remove-item-btn"
                    onClick={() => removeItem(item.id)}
                  >
                    <img src={ShoppingBagIcon} alt="Remove" />
                  </button>
                </div>
              ))}
            </div>

            <div className="order-calculations">
              <div className="calc-row">
                <span>Subtotal:</span>
                <span>{subtotal} ₴</span>
              </div>
              <div className="calc-row">
                <span>Delivery:</span>
                <span>{delivery} ₴</span>
              </div>
              <div className="calc-row discount">
                <span>Discount:</span>
                <span>10%</span>
              </div>
              <div className="calc-row total">
                <span>TOTAL:</span>
                <span>{total} ₴</span>
              </div>
            </div>

            <div className="add-card-section">
              <button 
                className="add-card-btn"
                onClick={toggleCard}
              >
                {isCardAdded ? "- Remove card" : "+ Add a card"}
              </button>
              <span className="card-price">+ 50 ₴</span>
            </div>

            <textarea
              className="message-input"
              placeholder="Input your message"
              name="message"
              value={formData.message}
              onChange={handleInputChange}
            />

            <label className="checkbox-label">
              <input
                type="checkbox"
                name="noCall"
                checked={formData.noCall}
                onChange={handleInputChange}
              />
              <span>Do not call me, I am certain with my order</span>
            </label>

            <button className="confirm-btn" onClick={handleConfirmOrder}>
              CONFIRM ORDER
            </button>
          </div>
        </div>
      </main>

      <Footer />
    </div>
  );
};

export default OrderPlacement;