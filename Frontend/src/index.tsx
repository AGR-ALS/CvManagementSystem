import React from 'react';
import ReactDOM from 'react-dom/client';
import './i18n';
import App from './App';
import {AuthProvider} from "./Contexts/AuthContext";

const root = ReactDOM.createRoot(
    document.getElementById('root') as HTMLElement
);

root.render(
    <React.StrictMode>
        <AuthProvider>
            <App/>
        </AuthProvider>
    </React.StrictMode>
);