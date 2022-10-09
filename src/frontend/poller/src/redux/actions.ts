import { Poll } from "../scripts/objects";

export const setPolls = (polls: Array<Poll>) => ({
    type: 'SET_POLLS' as const,
    polls,
})