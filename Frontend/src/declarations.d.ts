declare module "*.css";

declare namespace NodeJS {
    interface ProcessEnv {
        REACT_APP_API_URL?: string;
        REACT_APP_ADMIN_ROLE?: string;
        REACT_APP_RECRUITER_ROLE?: string;
        REACT_APP_REGULAR_ROLE?: string;
    }
}

declare var process: {
    env: NodeJS.ProcessEnv;
};
