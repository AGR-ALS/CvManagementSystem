import {BrowserRouter} from "react-router";
import AppShell from "./Components/AppShell/AppShell";

export default function App() {
    return (
        <BrowserRouter>
            <AppShell/>
        </BrowserRouter>
    );
};
