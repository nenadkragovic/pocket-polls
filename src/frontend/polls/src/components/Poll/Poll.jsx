import { React, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import * as style from './style/poll.scss';
import * as http from '../../scripts/http';
import Container from '@mui/material/Container';
import Box from '@mui/material/Box';
import { useState } from 'react';
import Typography from '@mui/material/Typography';
import LinearProgress from '@mui/material/LinearProgress';
import Button from '@mui/material/Button';

function Poll() {
	const { id } = useParams();
	const [poll, setPoll] = useState({});
	const [answers, setAnswers] = useState({
		questions: [],
		progress: 50,
		currentQuestion: 0,
		numberOfQuestions: 0,
		completed: false
	});

	useEffect(() => {
		http.request('polls/' + id, 'GET', null)
			.then(result => {
				setPoll(result.data);
				let questions = [];
				result.data.yesNoQuestions.map((item) => questions.push(item));
				result.data.singleChoiceQuestions.map((item) => questions.push(item));
				result.data.textQuestions.map((item) => questions.push(item));
				result.data.multipleChoiceQuestions.map((item) => questions.push(item));
				setAnswers({
					...answers,
					questions: questions,
					numberOfQuestions: questions.length,
					progress: 0
				})
			})
			.catch(err => {
			});
	}, []);

	const nextQuestion = () => {
		let completed = answers.currentQuestion >= (answers.numberOfQuestions - 1)
		setAnswers({
			...answers,
			currentQuestion: completed ? answers.currentQuestion : (1 + answers.currentQuestion),
			completed: completed,
			progress: (answers.currentQuestion > 0 ? ((1 + answers.currentQuestion)) : 1)/ answers.numberOfQuestions * 100
		})
		console.log(answers.currentQuestion);
	}

	const prevQuestion = () => {
		setAnswers({
			...answers,
			currentQuestion: (answers.currentQuestion - 1) > 0 ? (answers.currentQuestion - 1) : 0,
			progress: (answers.currentQuestion > 0 ? ((1 + answers.currentQuestion)) : 1)/ answers.numberOfQuestions * 100
		})
		console.log(answers.currentQuestion);
	}

	function Question(props) {
		switch(props.question.questionType) {
		  case 0:
			return <p>YesNoQuestion</p>
		  case 1:
			return <p>SingleChoice</p>
		  case 2:
			return <p>MultipleChoice</p>
		  default:
			return <p>Text question</p>
		}
	}
	return (
		<Container style={style} className="container">
			<Typography className="poll-name" textAlign="center">{poll.name}</Typography>
			{
				answers.numberOfQuestions > 0 ?
					<div>
						<LinearProgress className="progress" variant="determinate" value={answers.progress} />
						<Box className="question">
							<Typography className="questionText" textAlign="left">#{answers.currentQuestion}</Typography>
							<Typography className="questionText" textAlign="center">{answers.questions[answers.currentQuestion].questionText}</Typography>
							<Question question={answers.questions[answers.currentQuestion]}></Question>
						</Box>
						<Box className="buttons">
						<Button variant="outlined" onClick={prevQuestion}>Previous</Button>
						<Button variant="outlined" onClick={nextQuestion}>Next</Button>
					</Box>
					</div>:
					<Box className="question">
						<Typography className="questionText" textAlign="center">No questions</Typography>
					</Box>
			}
		</Container>
	);
};

export default Poll;
