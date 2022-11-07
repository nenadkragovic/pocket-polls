import React from 'react';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box'
import { Chart } from "react-google-charts";
import * as helper from '../../scripts/helper';

export function PollAnswers (props) {

    var [count, setCount] = React.useState(1);

    const getSingleChoiceChartData = (choices, total) => {
        let data = [["SingleAnswers", "Single choice Answers"]];
        for (let i in choices)
            data.push([choices[i].choiceName, Math.ceil(choices[i].total/total*100)])
        return data;
    }

    const getMultipleChoiceChartData = (choices, total) => {
        let colors = ["#e1f5fe", "#b3e5fc", "#81d4fa", "#4fc3f7", "#29b6f6", "#03a9f4", "#039be5", "#0288d1", "#0277bd", "#01579b"]
        let data = [ ["Element", "Density", { role: "style" }]];
        for (let i in choices)
            data.push([choices[i].choiceName, choices[i].total, colors[i%10]])
        return data;
    }

    return (
        <>
            {
                props.poll == null ?
                <h4>Selet poll to view data</h4> :
                <>
                     <h4>{props.poll.name}</h4>
                     {
                        props.answers?.yesNoAnswers.map((item) => (
                            <Box sx={{ boxShadow: 3 }} key={item.id} className="answers">
                                <h5 className="question-text">
                                    <span>{count++}. </span>
                                    <span>{item.questionText}</span>
                                </h5>
                                <Chart
                                    className='pie'
                                    chartType="PieChart"
                                    data={[
                                        ["Answers", "Yes/No Answers"],
                                        [ 'Yes', Math.ceil(item.yesCount/item.total*100) ],
                                        [ 'No', Math.ceil((item.total - item.yesCount)/item.total*100)],
                                    ]}
                                    options= {{
                                        title: `Total: ${item.total}`,
                                        is3D: false,
                                    }}
                                />
                            </Box>
                        ))
                     }
                     {
                        props.answers?.singleChoiceAnswers.map((item) => (
                            <Box sx={{ boxShadow: 3 }} key={item.id} className="answers">
                                <h5 className="question-text">
                                    <span>{count++}. </span>
                                    <span>{item.questionText}</span>
                                </h5>
                                <Chart
                                    className='pie'
                                    chartType="PieChart"
                                    data={getSingleChoiceChartData(item.choices, item.total)}
                                    options= {{
                                        title: `Total: ${item.total}`,
                                        is3D: false,
                                    }}
                                />
                            </Box>
                        ))
                     }
                     {
                        props.answers?.multipleChoiceAnswers.map((item) => (
                            <Box sx={{ boxShadow: 3 }} key={item.id} className="answers">
                                <h5 className="question-text">
                                    <span>{count++}. </span>
                                    <span>{item.questionText}</span>
                                </h5>
                                <div className="question-answers">
                                    <Chart className="pie" chartType="ColumnChart" width="100%" height="400px" data={getMultipleChoiceChartData(item.choices, item.total)} />
                                </div>
                            </Box>
                        ))
                     }
                     {
                        props.answers?.textAnswers.map((item) => (
                            <Box sx={{ boxShadow: 3 }} key={item.id} className="answers">
                                <h5 className="question-text">
                                    <span>{count++}. </span>
                                    <span>{item.questionText}</span>
                                </h5>
                                <div className="question-answers">
                                {
                                    item.answers?.map((answer, index) => (
                                        <p className="text-answer">
                                            <span>{index + 1}. </span>
                                            {answer}
                                        </p>
                                    ))
                                }
                                </div>
                            </Box>
                        ))
                     }
                </>
            }
        </>
    )
}