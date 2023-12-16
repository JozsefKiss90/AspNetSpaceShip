import "./Home.css";

export default function TermsAndConditions() {
    return (
        <div className="home-container">
            <div className="home-message">
                <h2>Terms & Conditions</h2>
                <div className="code">
                    <div>{"if (condition) {"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>{"} else if (condition) {"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>{"} else {"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>&emsp;&emsp;{"term;"}</div>
                    <div>{"}"}</div>
                </div>
            </div>
        </div>
    );
}
