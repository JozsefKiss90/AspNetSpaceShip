import { useRouteError } from "react-router-dom";
import GenericErrorPage from "./GenericErrorPage";

export default function ErrorPage() {
    const error = useRouteError();
    console.error(error);

    return <GenericErrorPage />;
}