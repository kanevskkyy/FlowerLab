import React from "react";
import { Swiper, SwiperSlide } from "swiper/react";
import { Navigation } from "swiper/modules";
import "swiper/css";
import "swiper/css/navigation";

import arrowLeft from "../../../assets/icons/arrow-left.svg";
import arrowRight from "../../../assets/icons/arrow-right.svg";

const BestWithSlider = ({ gifts, onGiftClick }) => {
  if (!gifts || gifts.length === 0) return null;

  return (
    <section className="recommendations-section">
      <h2 className="section-title">Best with</h2>

      <div className="recommendations-slider-container">
        <div className="custom-prev">
          <img src={arrowLeft} alt="Previous" />
        </div>
        <Swiper
          modules={[Navigation]}
          navigation={{
            prevEl: ".custom-prev",
            nextEl: ".custom-next",
          }}
          spaceBetween={20}
          slidesPerView={1.25}
          breakpoints={{
            550: {
              slidesPerView: 1.8,
              spaceBetween: 20,
            },
            950: {
              slidesPerView: 2.5,
              spaceBetween: 30,
            },
            1200: {
              slidesPerView: 3,
              spaceBetween: 40,
            },
          }}
          className="recommendations-swiper">
          {gifts.map((gift) => (
            <SwiperSlide key={gift.id}>
              <div
                className="recommendation-card"
                onClick={() => onGiftClick(gift)}>
                <div className="rec-image">
                  <img src={gift.image} alt={gift.title} />
                </div>
                <div className="rec-details">
                  <p className="rec-title">{gift.title}</p>
                  <p className="rec-price">{gift.price}</p>
                </div>
              </div>
            </SwiperSlide>
          ))}
        </Swiper>
        <div className="custom-next">
          <img src={arrowRight} alt="Next" />
        </div>
      </div>
    </section>
  );
};

export default BestWithSlider;
