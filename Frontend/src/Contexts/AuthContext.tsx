import React, {createContext, useContext, useState, useEffect, useCallback, ReactNode} from "react";
import {UserRole} from "../models/UserRole";
import {IsUserLoggedIn, Logout} from "../Services/AuthService";
import {GetCurrentUserId, GetCurrentUserRole} from "../Services/UserService";

interface AuthState {
    role: UserRole | null;
    userId: string;
    isLoggedIn: boolean;
    isLoading: boolean;
}

interface AuthContextValue extends AuthState {
    refresh: () => Promise<void>;
    logout: () => Promise<void>;
    login: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({children}: { children: ReactNode }) {
    const [state, setState] = useState<AuthState>({
        role: null,
        userId: "",
        isLoggedIn: false,
        isLoading: true,
    });

    const refresh = useCallback(async () => {
        setState(prev => ({...prev, isLoading: true}));
        try {
            const loggedIn = await IsUserLoggedIn();
            if (loggedIn) {
                const [userId, role] = await Promise.all([GetCurrentUserId(), GetCurrentUserRole()]);
                setState({role, userId, isLoggedIn: true, isLoading: false});
            } else {
                setState({role: null, userId: "", isLoggedIn: false, isLoading: false});
            }
        } catch {
            setState({role: null, userId: "", isLoggedIn: false, isLoading: false});
        }
    }, []);

    const logout = useCallback(async () => {
        await Logout();
        setState({role: null, userId: "", isLoggedIn: false, isLoading: false});
    }, []);

    const login = useCallback(async () => {
        await refresh();
    }, [refresh]);

    useEffect(() => {
        refresh();
    }, [refresh]);

    return (
        <AuthContext.Provider value={{...state, refresh, logout, login}}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth(): AuthContextValue {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("useAuth must be used within AuthProvider");
    return ctx;
}
