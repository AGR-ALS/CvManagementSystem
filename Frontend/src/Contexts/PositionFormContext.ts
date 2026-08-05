import {createContext, useContext} from "react";
import {AttributeDefinition} from "../models/AttributeDefinition";

export interface PositionFormContextType {
    technologyOptions: { value: string; label: string }[];
    onSearchTechnologies: (query: string) => Promise<void>;
    attributeDefinitions: AttributeDefinition[];
    onGenerateCv: () => Promise<void>;
    onGenerateApiToken: (positionId: string) => Promise<string>;
}

export const PositionFormContext = createContext<PositionFormContextType | undefined>(undefined);

export function usePositionFormContext() {
    const context = useContext(PositionFormContext);
    if (!context) {
        throw new Error("PositionFormContext must be defined");
    }
    return context;
}
