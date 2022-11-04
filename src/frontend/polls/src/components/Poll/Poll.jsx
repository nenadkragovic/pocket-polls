import { React, useEffect } from 'react';
import { Navigate, useParams } from 'react-router-dom';
import * as style from './style/poll.scss';
import * as http from '../../scripts/http';
import Container from '@mui/material/Container';
import Box from '@mui/material/Box';
import { useState } from 'react';
import Typography from '@mui/material/Typography';
import LinearProgress from '@mui/material/LinearProgress';
import Button from '@mui/material/Button';
import Switch from '@mui/material/Switch';
import TextareaAutosize from '@mui/material/TextareaAutosize';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormControl from '@mui/material/FormControl';
import FormGroup from '@mui/material/FormGroup';
import Checkbox from '@mui/material/Checkbox';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useNavigate } from 'react-router-dom';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import * as validation from '../../scripts/validationHelper';
import { QuestionType } from '../../scripts/enums';

function Poll() {
	const navigate = useNavigate();

	const { id } = useParams();
	const [poll, setPoll] = useState({});
	const [questionsData, setQuestionsData] = useState({
		questions: [],
		progress: 0,
		currentQuestion: 0,
		numberOfQuestions: 0,
		completed: false
	});

	const [answers, setAnswers] = useState({
		yesNoAnswers: [],
		singleChoiceAnswers: [],
		multipleChoiceAnswers: [],
		textAnswers: [],
	});

	const [validationData, setValidationData] = useState({
		open: false,
		message: '',
		severity: 'success'
	});

	const handleCloseValidationMessage = () => {
		setValidationData({
			...validationData,
			open: false,
			message: '',
		})

		if (validationData.severity === 'success')
			navigate('/');
	}

	useEffect(() => {
		http.request('polls/' + id, 'GET', null)
			.then(result => {
				setPoll(result.data);
				let questions = [];
				let yesNoAnswers =[];
				let singleChoiceAnswers = [];
				let multipleChoiceAnswers = [];
				let textAnswers = [];

				result.data.yesNoQuestions.map((item) => {
					questions.push(item);
					yesNoAnswers.push({
						questionId: item.id,
						answer: false
					});
				});
				result.data.singleChoiceQuestions.map((item) => {
					questions.push(item);
					singleChoiceAnswers.push({
						questionId: item.id,
						choiceId: item.choices[0].id
					});
				});
				result.data.multipleChoiceQuestions.map((item) => {
					questions.push(item);
					multipleChoiceAnswers.push({
						questionId: item.id,
						choiceIds: []
					});
				});
				result.data.textQuestions.map((item) => {
					questions.push(item);
					textAnswers.push({
						questionId: item.id,
						answer: ''
					});
				});

				setQuestionsData({
					...questionsData,
					questions: questions,
					numberOfQuestions: questions.length,
					progress: 1 / questions.length * 100
				})

				setAnswers({
					yesNoAnswers: yesNoAnswers,
					singleChoiceAnswers: singleChoiceAnswers,
					multipleChoiceAnswers: multipleChoiceAnswers,
					textAnswers: textAnswers
				});
			})
			.catch(err => {
			});
	}, []);

	const nextQuestion = () => {
		let nextQuestion = questionsData.currentQuestion < (questionsData.numberOfQuestions - 1) ?
			(1 + questionsData.currentQuestion) : questionsData.currentQuestion;
		let completed = nextQuestion == (questionsData.numberOfQuestions - 1);
		let progress = (1 + nextQuestion) / questionsData.numberOfQuestions * 100;
		setQuestionsData({
			...questionsData,
			currentQuestion: nextQuestion,
			completed: completed,
			progress: progress
		})
	}

	const prevQuestion = () => {
		if (questionsData.currentQuestion == 0){
			navigate('/');
		}

		let nextQuestion = questionsData.currentQuestion > 0 ? (questionsData.currentQuestion - 1) : 0;
		let progress = (1 + nextQuestion) / questionsData.numberOfQuestions * 100;
		let completed = nextQuestion == (questionsData.numberOfQuestions - 1);
		setQuestionsData({
			...questionsData,
			currentQuestion: nextQuestion,
			progress: progress,
			completed: completed
		})
	}

	const submit = () => {
		http.request('answers/' + poll.id, 'POST', answers)
			.then(result => {
				setValidationData({
					open: true,
					message: 'Answers submitted successfully!',
					severity: 'success'
				})
			})
			.catch(err => {
				var message = validation.getValidationMessage(err.response.data);

				setValidationData({
					open: true,
					message: message,
					severity: 'error'
				})
			});
	}

	const getYesNoAnswer = (questionId) => {
		let objIndex = answers.yesNoAnswers.findIndex((obj => obj.questionId === questionId));
		return answers.yesNoAnswers[objIndex].answer;
	}

	const setYesNoAnswer = (questionId, value) => {
		setAnswers(prevState => {
			let objIndex = prevState.yesNoAnswers.findIndex((obj => obj.questionId === questionId));
			let arr = prevState.yesNoAnswers;
			arr[objIndex].answer = value;
			return {
				 ...prevState,
				 yesNoAnswers: arr
			}
		 })
	}

	const getSingleChoiceAnswer = (questionId) => {
		let objIndex = answers.singleChoiceAnswers.findIndex((obj => obj.questionId === questionId));
		return answers.singleChoiceAnswers[objIndex].choiceId;
	}
	const setSingleChoiceAnswer = (questionId, choiceId) => {
		console.log(choiceId);
		setAnswers(prevState => {
			let objIndex = prevState.singleChoiceAnswers.findIndex((obj => obj.questionId === questionId));
			let arr = prevState.singleChoiceAnswers;
			arr[objIndex].choiceId = choiceId;
			return {
				 ...prevState,
				 singleChoiceAnswers: arr
			}
		 })
	}

	const getMultipleChoiceStatus = (questionId, choiceId) => {
		let objIndex = answers.multipleChoiceAnswers.findIndex((obj => obj.questionId === questionId));
		return answers.multipleChoiceAnswers[objIndex]?.choiceIds.includes(choiceId);
	}

	const setMultipleChoiceStatus = (questionId, choiceId, add) => {
		console.log(choiceId);
		setAnswers(prevState => {
			let objIndex = prevState.multipleChoiceAnswers.findIndex((obj => obj.questionId === questionId));
			let arr = prevState.multipleChoiceAnswers;
			add ? arr[objIndex].choiceIds.push(choiceId) : (arr[objIndex].choiceIds = arr[objIndex].choiceIds.filter(function(value, index, arr){
				return value !== choiceId;
			}));

			return {
				 ...prevState,
				 multipleChoiceAnswers: arr
			}
		 })
	}

	const getTextAnswer = (questionId) => {
		let objIndex = answers.textAnswers.findIndex((obj => obj.questionId === questionId));
		return answers.textAnswers[objIndex].answer;
	}

	const setTextAnswer = (questionId, value) => {
		setAnswers(prevState => {
			let objIndex = prevState.yesNoAnswers.findIndex((obj => obj.questionId === questionId));
			let arr = prevState.textAnswers;
			arr[objIndex].answer = value;
			return {
				 ...prevState,
				 textAnswers: arr
			}
		 })
		 console.log(answers)
	}

	function Question(props) {
		switch(props.question.questionType) {
		  case QuestionType.YesNo:
			return <div className='answer'>No <Switch checked={getYesNoAnswer(props.question.id)} onChange={(e) => setYesNoAnswer(props.question.id, e.target.checked)}/> Yes </div>
		  case QuestionType.SingleChoice:
			return <FormControl>
						<RadioGroup
							aria-labelledby="demo-radio-buttons-group-label"
							defaultValue={props.question.choices[0].name}
							name="radio-buttons-group"
							value = {getSingleChoiceAnswer(props.question.id)}
							onChange={(e) => setSingleChoiceAnswer(props.question.id, e.target.value)}
						>
							{
								props.question.choices.map((choice) => (
									<FormControlLabel key={choice.id} value={choice.id} control={<Radio />} label={choice.name} />
								))
							}
					</RadioGroup>
				</FormControl>
		  case QuestionType.MultipleChoice:
			return <FormGroup>
					{
						props.question.choices.map((choice) => (
							<FormControlLabel key={choice.id} value={choice.id} label={choice.name}
								control={<Checkbox
											checked={getMultipleChoiceStatus(props.question.id, choice.id)}
											onChange={(e) => setMultipleChoiceStatus(props.question.id, choice.id, e.target.checked)}
											/>}  />
						))
					}
				   </FormGroup>
		  default:
			return <div className='answer'>
						<TextareaAutosize
							defaultValue = {getTextAnswer(props.question.id)}
							onBlur= {(e) => {setTextAnswer(props.question.id, e.target.value)}}
							aria-label="Write your answer"
							minRows={3}
							placeholder=""
							style={{ width: '100%', maxHeight: '20rem', height: '10rem' }}
						/>
					</div>
		}
	}
	return (
		<Container style={style} className="container">
			<Snackbar
				className='validation'
				open={validationData.open}
				autoHideDuration={6000}
				onClose={handleCloseValidationMessage}>
					<MuiAlert severity={validationData.severity} elevation={6} variant="filled" onClose={handleCloseValidationMessage}>
						{validationData.message}
					</MuiAlert>
			</Snackbar>
			{
				questionsData.numberOfQuestions > 0 ?
				<>
					<div className='progress-box'>
						<Typography className="poll-name" textAlign="center">{poll.name}</Typography>
						<LinearProgress className="progress" variant="determinate" value={questionsData.progress} />
						<Typography className="questionText" textAlign="center">({questionsData.currentQuestion + 1}/{questionsData.numberOfQuestions})</Typography>
					</div>
					<div className="question">
						<Typography className="questionText" textAlign="center">{questionsData.questions[questionsData.currentQuestion].questionText}</Typography>
						<Question question={questionsData.questions[questionsData.currentQuestion]}></Question>
					</div>
					<div className="buttons">
						<Button variant="outlined" onClick={prevQuestion}><ArrowBackIcon></ArrowBackIcon></Button>
						{
							questionsData.completed ?
							<Button variant="outlined" onClick={submit}>Submit</Button> :
							<Button variant="outlined" onClick={nextQuestion}><ArrowForwardIcon></ArrowForwardIcon></Button>
						}
					</div>
				</> :
				<>
					<div className="question">
						<Typography className="poll-name" textAlign="center">{poll.name}</Typography>
						<Typography className="questionText" textAlign="center">No questions</Typography>
					</div>
					<div className="buttons">
						<Button variant="outlined" onClick={prevQuestion}><ArrowBackIcon></ArrowBackIcon></Button>
					</div>
				</>
			}
		</Container>
	);
};

export default Poll;
