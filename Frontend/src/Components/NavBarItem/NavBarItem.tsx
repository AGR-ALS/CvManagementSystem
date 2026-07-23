import {Link} from "react-router";
import {NavLink} from "../../models/NavLink";

interface Props {
    link: NavLink
}

export default function NavBarItem(props: Props) {
    return (
        <div>
            <Link
                to={props.link.url}
                className="text-dark text-decoration-none fw-medium px-3 py-3"
                onClick={(e) => {
                    if (props.link.onClick) {
                        e.preventDefault();
                        props.link.onClick();
                    }
                }}
            >
                {props.link.name}
            </Link>
        </div>
    )
}
