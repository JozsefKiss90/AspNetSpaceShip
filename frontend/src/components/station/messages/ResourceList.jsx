export default function ResourceList({ message, resources }) {
  return (
    <>
      <div>{message}</div>
      {Object.keys(resources).map((key) => {
        return (
          <div className="resource-row" key={key}>
            <img src={"/" + key.toLowerCase() + ".png"} alt={key} />
            <div>
              {key} : {resources[key]}
            </div>
          </div>
        );
      })}
    </>
  );
}
