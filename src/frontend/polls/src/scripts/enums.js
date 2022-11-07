export const QuestionType = {
    YesNo: 0,
    SingleChoice: 1,
    MultipleChoice: 2,
    Text: 3
}

export const questionTypeToString = (type) => {
    for (const [key, value] of Object.entries(QuestionType)) {
        if (value === type)
        return key.split(/(?=[A-Z])/).join(" ");
    }
}