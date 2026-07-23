import {Project} from "./Project";
import {ProfileData} from "./ProfileData";
import {AttributeValue} from "./AttributeValue";
import {CvBasicModel} from "./CvBasicModel";
import {UserRole} from "./UserRole";

export interface UserProfile {
    id: string;
    profileData?: ProfileData;
    role: UserRole;
    projects: Project[];
    attributeValues: AttributeValue[];
    version: number;
    email: string;
    isBlocked: boolean;
    cvs: CvBasicModel[]
}