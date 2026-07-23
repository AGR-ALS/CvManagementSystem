import {Technology} from "./Technology";

export interface Project {
    id: string;
    name: string;
    description: string;
    technologies: Technology[];
    version: number;
}