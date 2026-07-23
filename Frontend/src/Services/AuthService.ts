import {apiRequest} from "./apiClient";
import {LoginUserRequest} from "../models/LoginUserRequest";

const baseUrl = "/Users";
export const Login = async (request: LoginUserRequest): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/login`, {
        method: "POST",
        body: JSON.stringify(request),
    });
};

export const Register = async (request: LoginUserRequest): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/register`, {
        method: "POST",
        body: JSON.stringify(request),
    });
};

export const Logout = async (): Promise<void> => {
    await apiRequest<void>(`${baseUrl}/logout`, {
        method: "POST",
    });
};

export const IsUserLoggedIn = async (): Promise<boolean> => {
    try {
        return (await apiRequest<boolean>(`${baseUrl}/auth/status`, {
            method: "GET",
        })) ?? false;
    } catch {
        return false;
    }
};