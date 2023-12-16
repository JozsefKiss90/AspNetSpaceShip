export default function Scanner({scanner, onClick}) {
    return (
        <div className="row">
          <div>
            Scanner | lvl: {scanner.level} | efficiency: {scanner.efficiency}
          </div>
          <button
            className="button"
            onClick={() => onClick("SCANNER")}
            disabled={scanner.fullyUpgraded}
          >
            {scanner.fullyUpgraded ? "Max level" : "Upgrade"}
          </button>
        </div>
      );
}