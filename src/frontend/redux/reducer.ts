import { Poll, Question } from "../scripts/objects";
import * as actions from './actions'

export type ActionType<Action extends (...args: any[]) => { type: string }> = ReturnType<Action>

type ValueOf<T> = T[keyof T]
export type ActionsType<Module extends Record<string, (...args: any[]) => { type: string }>> = ValueOf<{ [K in keyof Module]: ActionType<Module[K]> }>

export interface PollsStore {
  polls: Array<Poll>
}

export const EMPTY_STORE: PollsStore = {
  polls: new Array<Poll>()
}

export const reducer = (state: PollsStore = EMPTY_STORE, action:  ActionsType<typeof actions>): PollsStore => {

  switch (action.type) {
    case 'SET_POLLS':
      return {
        ...state,
        polls: action.polls
      }
    default:
      return state
  }
}