import React from 'react';
import { useState } from 'react';
import  * as style from './style/createPoll.scss';
import Container from '@mui/material/Container';
import FormControl from '@mui/material/FormControl';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import IconButton from '@mui/material/IconButton';
import InputLabel from '@mui/material/InputLabel';
import OutlinedInput from '@mui/material/OutlinedInput';
import * as http from '../../scripts/http';
import { useNavigate } from 'react-router-dom';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import { QuestionType, questionTypeToString } from '../../scripts/enums';
import CloseIcon from '@mui/icons-material/Close';
import AddIcon from '@mui/icons-material/Add';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import * as validation from '../../scripts/validationHelper';

function CreatePoll() {
    const navigate = useNavigate();

    let initData = {
        name: '',
        description: '',
        questions: [],
        numberOfQustions: 0
    }

    const [data, setData] = useState(initData);

    let addQuestionInitData = {
        opened: false,
        text: '',
        type: QuestionType.YesNo,
        choiceToAdd: '',
        choices: []
    }

    const [addQuestionData, setAddQuestionData] = useState(addQuestionInitData);

    const [validationData, setValidationData] = useState({
        open: false,
        message: '',
    });

    const handleCloseValidationMessage = () => {
        setValidationData({
            open: false,
            message: '',
            severity: 'info'
        })
    }

    const handleChange = (prop) => (event) => {
        setData({ ...data, [prop]: event.target.value });
    };

    const handleAddQuestionChange = (prop) => (event) => {
        setAddQuestionData({ ...addQuestionData, [prop]: event.target.value });
    };

    const addChoice = () => {
        if (addQuestionData.choiceToAdd === '')
            return;
        if (addQuestionData.choices.includes(addQuestionData.choiceToAdd)){
            setValidationData({
                open: true,
                message: 'Choice already added.',
                severity: 'warning'
            })
            return;
        }
        let choices = addQuestionData.choices;
        choices.push({Name: addQuestionData.choiceToAdd, Description: ''})
        setAddQuestionData({
            ...addQuestionData,
            choices: choices,
            choiceToAdd: ''
        })
    }

    const removeChoice = (item) => {
        let choices = addQuestionData.choices;
        choices = choices.filter(function(value, index, arr){
            return value.Name !== item;
        });
        setAddQuestionData({
            ...addQuestionData,
            choices: choices,
            choiceToAdd: ''
        })
    }

    const openAddQuestionDialog = (open) => {
        setAddQuestionData({...addQuestionData, opened: open})
    }

    const addQuestion = () => {
        console.log(data);
        if (addQuestionData.text === '' || addQuestionData.text.length < 5){
            setValidationData({
                open: true,
                message: 'Question Text is not valid.',
                severity: 'warning'
            })
            return;
        }
        if ((addQuestionData.type == QuestionType.SingleChoice ||
            addQuestionData.QuestionType == QuestionType.MultipleChoice) &&
            addQuestionData.choices.length < 2
            ){
            setValidationData({
                open: true,
                message: 'You must add at least two choices.',
                severity: 'warning'
            })
            return;
        }
        var questions = data.questions;
        questions.push({
            Text: addQuestionData.text,
            QuestionType: addQuestionData.type,
            Choices: addQuestionData.choices
        })
        setData({
            ...data,
            questions: questions,
            numberOfQustions: questions.length
        });
        setTimeout(() => {setAddQuestionData(addQuestionInitData)}, 100);
        console.log(data)
    }

    const submit = async () => {
        if (data.questions == null || data.questions < 1){
            setValidationData({
                open: true,
                message: 'You must add questions.',
                severity: 'warning'
            })
            return;
        }

        if (data.name === '' || data.name.length < 5){
            setValidationData({
                open: true,
                message: 'Name is required.',
                severity: 'warning'
            })
            return;
        }

        await http.request("polls", 'POST', data)
                .then(result => {
                    setData(initData);
                    setValidationData({
                        open: true,
                        message: 'Poll created successfully!',
                        severity: 'success'
                    })
                }).catch(err => {
                    var message = validation.getValidationMessage(err.response.data);

                    setValidationData({
                        open: true,
                        message: message,
                        severity: 'error'
                    })
                });
    }

    function QuestionsPreview(props) {

        const [questionNumber, setQuestionNumber] = useState(0)

        const prevQuestion = () => {
            if (questionNumber > 0) {
                setQuestionNumber(questionNumber-1);
            }
        }

        const nextQuestion = () => {
            if (questionNumber < (props.questions.length -1)){
                setQuestionNumber(questionNumber+1);
            }
        }

        const removeQuestion = () => {
            var questions = data.questions;
            questions.splice(questionNumber, 1);
            setData({
                ...data,
                questions: questions,
                numberOfQustions: questions.length
            })
            prevQuestion();
        }

        return (
            <FormControl className="questions-preview">
                <Button variant="outlined" onClick={prevQuestion}><ArrowBackIcon></ArrowBackIcon></Button>
                {
                    data.questions[questionNumber] != null ?
                    <div style={{padding: '0.5rem'}}>
                        <h5 className='questions-name'>
                            <span>#{questionNumber + 1} ({questionTypeToString(data.questions[questionNumber].QuestionType)}):</span>
                            <span className='remove-question'><IconButton onClick={() => removeQuestion()} edge="end" aria-label="delete"><CloseIcon /></IconButton></span>
                        </h5>
                        <p>{data.questions[questionNumber].Text}</p>
                        <ul>
                            {
                                data.questions[questionNumber].Choices?.map((choice) => (
                                    <li key={choice.Name}>{choice.Name}</li>
                                ))
                            }
                        </ul>
                    </div> : null
                }
                <Button variant="outlined" onClick={nextQuestion}><ArrowForwardIcon></ArrowForwardIcon></Button>
            </FormControl>
        );
    }

    return (
        <>
        <Container style={style} className="create-poll-container">
            <FormControl className="form-control">
                <InputLabel htmlFor="outlined-adornment-name">Poll Name</InputLabel>
                <OutlinedInput
                    id="outlined-adornment-name"
                    value={data.name}
                    onChange={handleChange('name')}
                    aria-describedby="outlined-weight-helper-text"
                    label="Poll Name"
                />
            </FormControl>
            <FormControl className="form-control">
                <InputLabel htmlFor="outlined-adornment-description">Poll Description</InputLabel>
                <OutlinedInput
                    id="outlined-adornment-description"
                    value={data.description}
                    onChange={handleChange('description')}
                    aria-describedby="outlined-weight-helper-text"
                    label="Poll Description"
                />
            </FormControl>
            <Typography variant="h5" component="div" style={{ marginBottom: '1rem'}}>Questions ({data.numberOfQustions}):</Typography>
            {
                data.questions != null && data.questions.length > 0 ?
                    <QuestionsPreview questions={data.questions}></QuestionsPreview> : null
            }
            <Button variant="outlined" onClick={() => openAddQuestionDialog(true)}>Add new question</Button>

            {
                addQuestionData.opened ?
                <>
                    <hr style={{color: 'black', width: '100%'}}/>

                    <FormControl className="close-add-question-dialog">
                        <Typography variant="h5" component="div" style={{ marginBottom: '1rem'}}>Add question</Typography>
                        <CloseIcon onClick={() => openAddQuestionDialog(false)}></CloseIcon>
                    </FormControl>
                    <FormControl className="form-control">
                        <InputLabel htmlFor="outlined-adornment-text">Question text</InputLabel>
                        <OutlinedInput
                            id="outlined-adornment-text"
                            value={addQuestionData.text}
                            onChange={handleAddQuestionChange('text')}
                            aria-describedby="outlined-weight-helper-text"
                            label="Question text"
                        />
                    </FormControl>
                    <FormControl fullWidth>
                    <InputLabel id="select-label">Question Type</InputLabel>
                        <Select
                            labelId="select-label"
                            id="select"
                            value={addQuestionData.type}
                            label="Question Type"
                            onChange={handleAddQuestionChange('type')}
                        >
                            <MenuItem default value={QuestionType.YesNo}>Yes/No</MenuItem>
                            <MenuItem value={QuestionType.SingleChoice}>Single Choice</MenuItem>
                            <MenuItem value={QuestionType.MultipleChoice}>Multiple Choice</MenuItem>
                            <MenuItem value={QuestionType.Text}>Text</MenuItem>
                        </Select>
                    </FormControl>
                    {
                        (addQuestionData.type == QuestionType.SingleChoice || addQuestionData.type == QuestionType.MultipleChoice) ?
                        <>
                            <Typography variant="p" component="div" style={{ marginTop: '1rem', marginBottom: '1rem'}}>Choices:</Typography>
                            <List>
                                {
                                    addQuestionData.choices.map((item) => (
                                        <ListItem
                                            key={item.Name}
                                            secondaryAction= {
                                                <IconButton onClick={() => removeChoice(item.Name)} edge="end" aria-label="delete"><CloseIcon /></IconButton>
                                            }
                                          >
                                            <ListItemText primary={item.Name}/>
                                        </ListItem>
                                    ))
                                }
                            </List>
                            <FormControl className="choice-control">
                                <OutlinedInput
                                    id="outlined-adornment-choice"
                                    value={addQuestionData.choiceToAdd}
                                    onChange={handleAddQuestionChange('choiceToAdd')}
                                    aria-describedby="outlined-weight-helper-text"
                                />
                                <Button variant="outlined" onClick={addChoice}><AddIcon></AddIcon></Button>
                            </FormControl>
                        </> : null
                    }
                    <FormControl style={{marginTop: '1rem'}}>
                        <Button variant="outlined" onClick={addQuestion}>Add</Button>
                    </FormControl>
                    <hr style={{color: 'black', width: '100%'}}/>
                </> : null
            }
            <FormControl style={{marginTop: '1rem'}}>
                <Button variant="contained" onClick={submit}>Submit Poll</Button>
            </FormControl>
        </Container>
        <Snackbar
            className='validation'
            open={validationData.open}
            autoHideDuration={6000}
            onClose={handleCloseValidationMessage}>
                <MuiAlert severity={validationData.severity} elevation={6} variant="filled" onClose={handleCloseValidationMessage}>
                    {validationData.message}
                </MuiAlert>
        </Snackbar>
        </>
    );
};

export default CreatePoll;
