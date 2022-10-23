export enum QuestionType {
    YesNoQuestion,
    SingleChoice,
    MultipleChoice,
    TextAnswer,
};

export class AnswerChoice
{
    name: string;
    value: string;

    constructor(name: string, value: string) {
        this.name = name;
        this.value = value;
    }
}

export class Question {
    id: number;
    type: QuestionType;
    question: string;

    constructor(id: number, type: QuestionType, question: string) {
        this.id = id;
        this.type = type;
        this.question = question;
    }
}

export class YesNoQuestion extends Question {
    answer: boolean;

    constructor (id: number, question: string, defaultAnswer: boolean = false) {
        super(id, QuestionType.YesNoQuestion, question);
        this.answer = defaultAnswer;
    }
}

export class SingleChoiceQuestion extends Question {
    answer: string;
    choices: AnswerChoice[];

    constructor (id: number, question: string, defaultAnswer: string, choices: AnswerChoice[]) {
        super(id, QuestionType.YesNoQuestion, question);
        this.answer = defaultAnswer;
        this.choices = choices;
    }

    setAnswer(answer: string) {
        this.answer = answer;
    }
}

export class MultipleChoiceQuestion extends Question {
    answers: string[];
    choices: AnswerChoice[];

    constructor (id: number, question: string, defaultAnswers: string[], choices: AnswerChoice[]) {
        super(id, QuestionType.YesNoQuestion, question);
        this.answers = defaultAnswers;
        this.choices = choices;
    }

    setAnswers(answers: string[]) {
        this.answers = answers;
    }

    addAnswers(answer: string) {
        this.answers.push(answer);
    }
}

export class TextQuestion extends Question {
    answer: string;

    constructor (id: number, question: string) {
        super(id, QuestionType.YesNoQuestion, question);
        this.answer = '';
    }
}

export class Poll {
    name: string;
    description: string;
    image: string;
    questions: Question[];

    constructor(){
        this.name = '';
        this.description = '';
        this.image = '';
        this.questions = new Array<Question>();
    }

    addQuestion(question: Question){
        this.questions.push(question);
    }

    addQuestions(questions: Array<Question>){
        questions.forEach(element => {
            this.questions.push(element);
        });
    }
}