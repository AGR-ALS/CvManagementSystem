import {useEffect, useRef} from "react";
import {CONFIG} from "../config";

export function useAutoSave<T>(
    data: T | null,
    saveFunction: (data: T, redirect: boolean) => Promise<void>,
    delay: number = CONFIG.AUTO_SAVE_DELAY,
) {
    const timer = useRef<number | null>(null);
    const previousData = useRef<T | null>(null);

    useEffect(() => {
        if (!data) {
            return;
        }

        if (JSON.stringify(previousData.current) === JSON.stringify(data)) {
            return;
        }

        previousData.current = data;

        if (timer.current) {
            clearTimeout(timer.current);
        }

        timer.current = setTimeout(async () => {
            await saveFunction(data, false);
        }, delay);

        return () => {
            if (timer.current) {
                clearTimeout(timer.current);
            }
        };

    }, [data]);
}