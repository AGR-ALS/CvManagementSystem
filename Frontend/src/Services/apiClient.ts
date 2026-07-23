import {NavigateFunction} from "react-router";

let navigate: NavigateFunction | null = null;
let currentPath = "";

export const setNavigate = (nav: NavigateFunction) => {
    navigate = nav;
};

export const setCurrentPath = (path: string) => {
    currentPath = path;
};

const BASE_URL = process.env.REACT_APP_API_URL;

export async function apiRequest<T = any>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const isFormData = options.body instanceof FormData;
    const hasBody = options.body != null;

    const headers = isFormData
        ? {...options.headers}
        : hasBody
            ? {"Content-Type": "application/json", ...options.headers}
            : {...options.headers};

    const url = `${BASE_URL}${endpoint.startsWith("/") ? endpoint : `/${endpoint}`}`;

    const response = await fetch(url, {
        credentials: "include",
        ...options,
        headers,
    });

    if (response.status === 403) {
        try {
            const statusRes = await fetch(`${BASE_URL}/users/block-status`, {credentials: "include"});
            const blocked = await statusRes.json();
            if (blocked && navigate) {
                navigate("/blocked");
            }
        } catch {
            console.error("403 Forbidden:", endpoint);
            throw new Error("Forbidden");
        }

    }

    if (response.status === 401) {
        const text = await response.text();
        let message = "Unauthorized";
        try {
            const body = JSON.parse(text);
            if (body?.detail) message = body.detail;
        }
        catch {
            throw new Error(message);
        }
        finally {
            if (!currentPath.includes("/authentication")) {
                if (navigate) navigate("/authentication");
            }
        }
    }

    if (!response.ok) {
        const text = await response.text();
        console.error(`HTTP ${response.status} for ${endpoint}:`, text || "No response body");
        throw new Error(text || `HTTP Error: ${response.status}`);
    }

    const text = await response.text();
    if (!text) {
        return undefined as T;
    }

    const contentType = response.headers.get("content-type");
    if (contentType?.includes("application/json")) {
        return JSON.parse(text) as T;
    }

    return text as unknown as T;
}
