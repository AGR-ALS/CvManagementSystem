import {createContext, useContext} from "react";
import {Cv} from "../models/Cv";
import {Project} from "../models/Project";
import {CvAttributeRow} from "../Components/CvAttributeTable/CvAttributeTable";
import {AttributeDataType} from "../models/AttributeDataType";
import {CreateUpdateAttributeValueRequest} from "../models/CreateUpdateAttributeValueRequest";

export interface CvFormContextType {
    cv: Cv;
    photo?: string;
    canEdit: boolean;
    regular: boolean;
    liked: boolean;
    isViewMode: boolean;
    projects: Project[];
    selectorOpen: boolean;
    attributeOpen: boolean;
    editingRow: CvAttributeRow | null;
    loadAttributeImage: (key: string) => Promise<string>;
    save: () => Promise<void>;
    remove: () => Promise<void>;
    saveAttribute: (type: AttributeDataType, data: CreateUpdateAttributeValueRequest) => Promise<void>;
    toggleLike: () => Promise<void>;
    publish: () => Promise<void>;
    onEdit: () => void;
    onCancel: () => void;
    openSelector: () => void;
    closeSelector: () => void;
    addProjects: (ids: string[]) => void;
    removeProject: (id: string) => void;
    openAttribute: (row: CvAttributeRow) => void;
    closeAttribute: () => void;
}

export const CvFormContext = createContext<CvFormContextType | undefined>(undefined);

export function useCvFormContext() {
    const ctx = useContext(CvFormContext);
    if (!ctx) {
        throw new Error("CvFormContext must be defined");
    }
    return ctx;
}
