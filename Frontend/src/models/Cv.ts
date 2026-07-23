import {UserProfile} from "./UserProfile";
import {Position} from "./Position";
import {Project} from "./Project";

export interface Cv {
    id: string;
    user: UserProfile;
    position: Position;
    projects: Project[];
    likes: number;
    published: boolean;
    version: number;
}