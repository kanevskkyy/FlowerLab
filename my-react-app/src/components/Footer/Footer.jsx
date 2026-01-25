import LocationIcon from "../../assets/icons/location-icon.svg";
import PhoneIcon from "../../assets/icons/phone-icon.svg";
import InstagramIcon from "../../assets/icons/instagram-icon.svg";
import TelegramIcon from "../../assets/icons/telegram-icon.svg";
import "./Footer.css";

const Footer = () => {
  return (
    <footer className="footer">
      <div className="footer-item footer-location">
        <img
          src={LocationIcon}
          alt="Location"
          className="footer-icon location-icon"
        />
        <div className="footer-text">
          <a
            href="https://maps.app.goo.gl/11uTt4nTxqpv2K3w5"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-text">
            <p>м. Чернівці, вул Герцена 2а,</p>
          </a>
          <a
            href="https://maps.app.goo.gl/myw4J2CtWA9AGVuj6"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-text">
            <p>вул Васіле Александрі, 1</p>
          </a>
        </div>
      </div>

      <div className="footer-item footer-phone">
        <img src={PhoneIcon} alt="Phone" className="footer-icon phone-icon" />
        <a href="tel:+380501591912" className="footer-link-single">
          <div className="footer-text">
            <p>+38 050 159 19 12</p>
          </div>
        </a>
      </div>

      <div className="footer-col">
        <div className="footer-item footer-instagram">
          <img
            src={InstagramIcon}
            alt="Instagram"
            className="footer-icon instagram-icon"
          />
          <a
            href="https://www.instagram.com/flowerlab_vlada/"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-single">
            <div className="footer-text">
              <p>@flowerlab_vlada</p>
            </div>
          </a>
        </div>

        <div className="footer-item footer-telegram">
          <img
            src={TelegramIcon}
            alt="Telegram"
            className="footer-icon telegram-icon"
          />
          <a
            href="https://t.me/flower_lab_vlada"
            target="_blank"
            rel="noopener noreferrer"
            className="footer-link-single">
            <div className="footer-text">
              <p>@flower_lab_vlada</p>
            </div>
          </a>
        </div>
      </div>
    </footer>
  );
};

export default Footer;
