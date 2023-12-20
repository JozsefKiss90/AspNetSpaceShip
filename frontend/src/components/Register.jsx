import { useNavigate, useOutletContext } from "react-router-dom";
import "./Login&Register.css";
import { useEffect, useState } from "react";
import useHandleFetchError from "../hooks/useHandleFetchError";
import { useNotificationsDispatch } from "./notifications/NotificationContext";

export default function Register() {
  const { user, update, setUpdate } = useOutletContext();
  const navigate = useNavigate();
  const handleFetchError = useHandleFetchError();
  const notifDispatch = useNotificationsDispatch();
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (user !== null) {
      navigate("/");
    }
  }, [user, navigate]);

  const onSubmit = (e) => {
    e.preventDefault();
    setSubmitting(true);
    const formData = new FormData(e.target);
    const entries = [...formData.entries()];

    const credentials = entries.reduce((acc, entry) => {
      const [k, v] = entry;
      acc[k] = v;
      return acc;
    }, {});
    handleRegister(credentials);
  };

  async function handleRegister(formData) {
    try {
      const res = await fetch("/Auth/Register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formData),
      });
      if (res.ok) {
        const data = await res.json();
        if (data) {
          setUpdate(!update);
          navigate("/login");
        }
      } else {
        handleFetchError(res);
      }
    } catch (err) {
      console.error(err);
      notifDispatch({
        type: "generic error",
      });
    }
    setSubmitting(false);
  }

  return (
    <div className="lrform-container">
      <form onSubmit={onSubmit}>
        <h2>Register</h2>
        <input name="email" type="email" required placeholder="Email"></input>
        <input name="username" type="text" required placeholder="Username"></input>
        <input name="password" type="Password" required placeholder="password"></input>
        <div>
          <input type="checkbox" required />
          <span>
            I accept the{" "}
            <a className="clickable" href="/terms" target="blank">
              Terms and Conditions
            </a>{" "}
            & <br />
            <a className="clickable" href="/privacy" target="blank">
              Privacy Policy
            </a>
          </span>
        </div>
        <button className="button" type="Submit" disabled={submitting}>
          Register
        </button>
      </form>
    </div>
  );
}
