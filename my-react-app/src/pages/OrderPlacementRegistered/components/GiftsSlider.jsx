import React, { useState } from "react";
import { Swiper, SwiperSlide } from "swiper/react";
import { Navigation, Pagination } from "swiper/modules";
import "swiper/css";
import "swiper/css/navigation";
import "swiper/css/pagination";

const GiftsSlider = ({ gifts, giftsLoading, selectedGifts, toggleGift }) => {
  const [prevEl, setPrevEl] = useState(null);
  const [nextEl, setNextEl] = useState(null);

  return (
    <section className="form-section">
      <h2>Gifts</h2>
      {giftsLoading ? (
        <div>Loading gifts...</div>
      ) : (
        <div className="gifts-slider-container">
          {/* External Navigation Buttons */}
          <div
            className="swiper-button-prev custom-prev-arrow"
            ref={(node) => setPrevEl(node)}></div>
          <div
            className="swiper-button-next custom-next-arrow"
            ref={(node) => setNextEl(node)}></div>

          <Swiper
            modules={[Navigation, Pagination]}
            spaceBetween={15}
            slidesPerView={1.2}
            navigation={{
              prevEl,
              nextEl,
            }}
            pagination={{ clickable: true }}
            breakpoints={{
              640: {
                slidesPerView: 2.2,
                spaceBetween: 20,
              },
              1024: {
                slidesPerView: 3,
                spaceBetween: 20,
              },
            }}
            className="gifts-swiper">
            {gifts.map((gift) => (
              <SwiperSlide key={gift.id}>
                <div className="gift-card">
                  <div className="gift-image">
                    <img
                      src={
                        gift.price
                          ? gift.imageUrl || gift.img || "/placeholder.png"
                          : gift.img
                      }
                      alt={gift.name}
                    />
                  </div>
                  <div className="gift-bottom">
                    <div className="gift-info">
                      <p className="gift-name">{gift.name}</p>
                      <p className="gift-price">{gift.price} ₴</p>
                    </div>
                    <button
                      type="button"
                      className={`add-gift-btn ${
                        selectedGifts.includes(gift.id) ? "active" : ""
                      }`}
                      onClick={() => toggleGift(gift.id)}>
                      +
                    </button>
                  </div>
                </div>
              </SwiperSlide>
            ))}
          </Swiper>
        </div>
      )}
    </section>
  );
};

export default GiftsSlider;
