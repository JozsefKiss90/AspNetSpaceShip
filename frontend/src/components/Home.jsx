import {useNavigate, useOutletContext} from "react-router";
import "./Home.css";

export default function Home() {
    const context = useOutletContext();
    const user = context.user;
    const navigate = useNavigate();
    return (
        <div className="home-container">
            <div className="home-message">
                {user === null ? (
                    <h2>Welcome, stranger!</h2>
                ) : (
                    <h2>{`Welcome, ${user.sub}!`}</h2>
                )}
                <div>
                    Are you ready to be the best Commander in the Minuend Galaxy?
                </div>
                <br/>
                <div>Start with a basic space station with one miner ship.</div>
                <div>Send ships on missions to gather resources.</div>
                <div>Spend your resources on storage and hangar upgrades.</div>
                <div>
                    Add more ships and upgrade their parts to improve efficiency.
                </div>
                <div>
                    Get on top of the Minuend Galaxy Space Station Commander Leaderboard!
                </div>
                <br/>
                {user === null ? (
                    <>
                        <div><span className="clickable" onClick={() => navigate("/register")}>Register</span> now to
                            join the fun!
                        </div>
                        <br/>
                        <div> If you already are a Commander, <span className="clickable"
                                                                    onClick={() => navigate("/login")}>log in</span>.
                        </div>
                        <br/>
                        <div> If you have issues logging in, click <span className="clickable"
                                                                         onClick={() => navigate("/issues")}>here.</span>
                        </div>
                    </>
                ) : (
                    <div> So what are you waiting for? Head to your{" "}
                        <span className="clickable" onClick={() => navigate("/station")}>station</span>, Commander!
                    </div>
                )}
                <br/>
                <div>
                    If you are bored, read our <span className="clickable" onClick={() => navigate("/terms")}>Terms and Conditions</span>{" "}
                    and <span className="clickable" onClick={() => navigate("privacy")}>Privacy Policy.</span>
                </div>
            </div>
        </div>
    );
}
