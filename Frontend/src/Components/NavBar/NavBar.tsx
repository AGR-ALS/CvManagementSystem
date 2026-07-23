import {useTranslation} from "react-i18next";
import {Select} from "antd";
import NavBarItem from "../NavBarItem/NavBarItem";
import {NavLink} from "../../models/NavLink";
import {Link} from "react-router";

interface Props {
    links: NavLink[]
}

export default function (props: Props) {
    const {t, i18n} = useTranslation();

    const changeLanguage = (lng: string) => {
        i18n.changeLanguage(lng);
    };

    return (
        <nav className="navbar navbar-light bg-white border-bottom">
            <div className="container-fluid">
                <Link to="/" className=" text-dark text-decoration-none">
                    {t("app.title")}
                </Link>
                <div className="d-flex align-items-center gap-2">
                    <i className="bi bi-globe"></i>
                    <Select
                        value={i18n.language}
                        onChange={changeLanguage}
                        options={[
                            {value: "en", label: "English"},
                            {value: "ru", label: "Russian"},
                        ]}
                    />
                    {props.links.map((link: NavLink) => (
                        <NavBarItem link={link} key={link.name}/>
                    ))}
                </div>
            </div>
        </nav>
    )
}
