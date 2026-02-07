import { useTranslation } from "react-i18next";
import LocationIcon from "../../assets/icons/location-icon.svg";
import PhoneIcon from "../../assets/icons/phone-icon.svg";
import InstagramIcon from "../../assets/icons/instagram-icon.svg";
import TelegramIcon from "../../assets/icons/telegram-icon.svg";
import ViberIcon from "../../assets/icons/viber-icon.svg";
import WhatsappIcon from "../../assets/icons/whatsapp-icon.svg";
import "./Footer.css";

const Footer = () => {
  const { t } = useTranslation();
  return (
    <footer className="footer">
      <div className="footer-item footer-location">
        <div className="footer-icon-wrapper">
          <img
            src={LocationIcon}
            alt="Location"
            className="footer-icon location-icon"
          />
        </div>
        <div className="footer-text">
          <a
            href="https://maps.app.goo.gl/11uTt4nTxqpv2K3w5"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-text">
            <p>{t("footer.address1")}</p>
          </a>
          <a
            href="https://maps.app.goo.gl/myw4J2CtWA9AGVuj6"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-text">
            <p>{t("footer.address2")}</p>
          </a>
        </div>
      </div>

      <div className="footer-item footer-phone">
        <div className="footer-icon-wrapper">
          <img src={PhoneIcon} alt="Phone" className="footer-icon phone-icon" />
        </div>
        <div className="contacts-numbers">
          <a href="tel:+380501591912" className="footer-link-single">
            <div className="footer-text">
              <p>+380 50 159 19 12</p>
            </div>
          </a>
          <a href="tel:+380976200144" className="footer-link-single">
            <div className="footer-text">
              <p>+380 97 620 01 44</p>
            </div>
          </a>
        </div>
      </div>

      <div className="footer-col footer-socials">
        <div className="footer-item footer-instagram">
          <a
            href="https://www.instagram.com/flowerlab_vlada/"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-single">
            <div className="footer-icon-wrapper">
              <img
                src={InstagramIcon}
                alt="Instagram"
                className="footer-icon instagram-icon"
              />
            </div>
          </a>
        </div>

        <div className="footer-item footer-telegram">
          <a
            href="https://t.me/flower_lab_vlada"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-single">
            <div className="footer-icon-wrapper">
              <img
                src={TelegramIcon}
                alt="Telegram"
                className="footer-icon telegram-icon"
              />
            </div>
          </a>
        </div>

        <div className="footer-item footer-viber">
          <a
            href="viber://chat?number=0976200144"
            className="footer-link-single">
            <div className="footer-icon-wrapper">
              <img
                src={ViberIcon}
                alt="Viber"
                className="footer-icon viber-icon"
              />
            </div>
          </a>
        </div>
        <div className="footer-item footer-whatsapp">
          <a
            href="https://wa.me/380501591912"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-single">
            <div className="footer-icon-wrapper">
              <img
                src={WhatsappIcon}
                alt="WhatsApp"
                className="footer-icon whatsapp-icon"
              />
            </div>
          </a>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
