import {Suspense, useCallback, useEffect, useMemo} from "react";
import {Route, Routes, useLocation, useNavigate} from "react-router";
import {useTranslation} from "react-i18next";
import {NavLink} from "../../models/NavLink";
import "../../App.css";
import NavBar from "../NavBar/NavBar";
import PositionsPage from "../../Pages/PositionsPage";
import AuthenticationPage from "../../Pages/AuthenticationPage";
import AttributeDefinitionPage from "../../Pages/AttributePage";
import ProfilePage from "../../Pages/ProfilePage";
import PositionPage from "../../Pages/PositionPage";
import CvPage from "../../Pages/CvPage";
import AllCvsPage from "../../Pages/AllCvsPage";
import UsersPage from "../../Pages/UsersPage";
import HomePage from "../../Pages/HomePage";
import BlockedPage from "../../Pages/BlockedPage";
import AccountConfirmPage from "../../Pages/AccountConfirmPage";
import {setCurrentPath, setNavigate} from "../../Services/apiClient";
import {isAdmin, isRecruiter, isRegular} from "../../utils/roles";
import {useAuth} from "../../Contexts/AuthContext";

export default function AppShell() {
    const navigate = useNavigate();
    const location = useLocation();
    const auth = useAuth();
    const {t} = useTranslation();

    useEffect(() => {
        setNavigate(navigate);
    }, [navigate]);

    useEffect(() => {
        setCurrentPath(location.pathname);
    }, [location.pathname]);

    const handleLogout = useCallback(async () => {
        navigate("/authentication");
        await auth.logout();
    }, [navigate, auth]);

    const links = useMemo<NavLink[]>(() => {
        if (auth.isLoading) {
            return [
                {name: t("nav.positions"), url: "/positions"},
                {name: t("nav.login"), url: "/authentication"},
            ];
        }

        if (!auth.isLoggedIn || !auth.role) {
            return [
                {name: t("nav.positions"), url: "/positions"},
                {name: t("nav.login"), url: "/authentication"},
            ];
        }

        const nextLinks: NavLink[] = [
            {name: t("nav.positions"), url: "/positions"},
        ];

        if (isRecruiter(auth.role) || isAdmin(auth.role)) {
            nextLinks.push(
                {name: t("nav.attributes"), url: "/attributes"},
                {name: t("nav.cvs"), url: "/cvs"},
            );
        }

        if (isAdmin(auth.role)) {
            nextLinks.push({name: t("nav.users"), url: "/users"});
        }

        if (isRegular(auth.role) || isRecruiter(auth.role) || isAdmin(auth.role)) {
            nextLinks.push({name: t("nav.profile"), url: "/profile?mode=view"});
            nextLinks.push({name: t("nav.logoff"), url: "#", onClick: handleLogout});
        }

        return nextLinks;
    }, [auth.isLoading, auth.isLoggedIn, auth.role, handleLogout, t]);

    return (
        <>
            <header className="app-shell__header">
                <NavBar links={links}/>
            </header>
            <main>
                <Suspense fallback={null}>
                    <Routes>
                    <Route path="/" element={<HomePage/>}/>
                    <Route path="/positions" element={<PositionsPage/>}/>
                    <Route path="/position" element={<PositionPage/>}/>
                    <Route path="/authentication" element={<AuthenticationPage/>}/>
                    <Route path="/attributes" element={<AttributeDefinitionPage/>}/>
                    <Route path="/profile" element={<ProfilePage/>}/>
                    <Route path="/cv" element={<CvPage/>}/>
                    <Route path="/cvs" element={<AllCvsPage/>}/>
                    <Route path="/users" element={<UsersPage/>}/>
                    <Route path="/blocked" element={<BlockedPage/>}/>
                    <Route path="/account-confirm" element={<AccountConfirmPage/>}/>
                    <Route path="*" element={<HomePage/>}/>
                </Routes>
                </Suspense>
            </main>
        </>
    );
}
