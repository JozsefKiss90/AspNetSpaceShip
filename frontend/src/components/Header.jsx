import { useNavigate } from "react-router";
import "./Header.css";
import useHandleFetchError from "../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "./notifications/NotificationContext";

const Header = ({ user, setUser }) => {
  const navigate = useNavigate();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();

  function logout() {
    localStorage.removeItem('jwt');
    setUser(null);
    navigate('/login');
  }

  const headerClass = window.location.pathname.includes("/admin/") ? "header header-blue" : "header";

  return (
    <>
      <div className={headerClass}>
        <p onClick={() => navigate("/")}>MINUEND SPACESHIP GAME</p>
        {user === null && (
          <span className="push" onClick={() => navigate("/login")}>
            Login
          </span>
        )}
        {(user !== null && user.role === "ADMIN") && (
          <span className="push" onClick={() => navigate("/admin/levels")}>
          Levels
        </span>
        )}
        {user !== null && (
          <span className={user.role === "ADMIN" ? "" : "push"} onClick={() => navigate("/station")}>
            {user.sub}
          </span>
        )}
        {user !== null && <span onClick={logout}>Log out</span>}
      </div>
    </>
  );
};

export default Header;
