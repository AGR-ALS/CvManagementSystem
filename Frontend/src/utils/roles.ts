import {UserRole} from "../models/UserRole";

const REACT_APP_ADMIN_ROLE = process.env.REACT_APP_ADMIN_ROLE;
const REACT_APP_RECRUITER_ROLE = process.env.REACT_APP_RECRUITER_ROLE;
const REACT_APP_REGULAR_ROLE = process.env.REACT_APP_REGULAR_ROLE;

export const getRoleName = (role?: UserRole | string | null): string => {
    if (!role) {
        return "";
    }
    if (typeof role === "string") {
        return role.toLowerCase();
    }
    return (role.name || "").toLowerCase();
};

export const isRegular = (role?: UserRole | string | null): boolean =>
    getRoleName(role) === REACT_APP_REGULAR_ROLE?.toLowerCase();

export const isRecruiter = (role?: UserRole | string | null): boolean =>
    getRoleName(role) === REACT_APP_RECRUITER_ROLE?.toLowerCase();

export const isAdmin = (role?: UserRole | string | null): boolean =>
    getRoleName(role) === REACT_APP_ADMIN_ROLE?.toLowerCase();
