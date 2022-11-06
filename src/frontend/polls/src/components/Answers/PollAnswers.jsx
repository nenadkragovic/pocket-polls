import React from 'react';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';

export function PollAnswers (props) {
    console.log(props.answers)
    return (
        <>
            {
                props.poll == null ?
                <h4>Selet poll to view data</h4> :
                <>
                     <Typography>{props.poll.name}</Typography>
                     {
                        props.answers?.yesNoAnswers.map((item) => (
                            <Box className="answers">
                                <p>{item.questionText}</p>
                                <p>{item.yesCount}/{item.total}</p>
                            </Box>
                        ))
                     }
                     {
                        props.answers?.singleChoiceAnswers.map((item) => (
                            <Box key={item.id} className="answers">
                                <p>{item.questionText}</p>
                                <p>Total: {item.total}</p>
                                {
                                    item.choices?.map((choice) => (
                                        <p>
                                            <span>{choice.choiceName}:</span>
                                            <span>{choice.total}/{item.total}</span>
                                        </p>
                                    ))
                                }
                            </Box>
                        ))
                     }
                     {
                        props.answers?.multipleChoiceAnswers.map((item) => (
                            <Box key={item.id} className="answers">
                                <p>{item.questionText}</p>
                                <p>Total: {item.total}</p>
                                {
                                    item.choices?.map((choice) => (
                                        <p>
                                            <span>{choice.choiceName}:</span>
                                            <span>{choice.total}/{item.total}</span>
                                        </p>
                                    ))
                                }
                            </Box>
                        ))
                     }
                     {
                        props.answers?.textAnswers.map((item) => (
                            <Box key={item.id} className="answers">
                                <p>{item.questionText}</p>
                                <p>Total: {item.total}</p>
                                {
                                    item.answers?.map((answer) => (
                                        <p>
                                            {answer}
                                        </p>
                                    ))
                                }
                            </Box>
                        ))
                     }
                </>
            }
        </>
    )
}