import { DirectLine } from 'botframework-directlinejs';
import * as React from 'react';
import { IWebChatProps } from './IWebChatProps';
import { escape } from '@microsoft/sp-lodash-subset';
import ReactWebChat from 'botframework-webchat';
import { WebPartContext } from '@microsoft/sp-webpart-base';



export default class WebChat extends React.Component<IWebChatProps, {}> {
  private directLine = new DirectLine({token:'r6uV_IRPDGQ.fA0nFdwxRWQD_h2wqREv9rua4NKF0XwFdF-6sjW0lPc'});
  constructor(props){
    super(props);
    let ctx:WebPartContext = this.props.context;
  }

  public componentDidMount(){
    this.directLine.postActivity({from:{id:`mfinkenhagen`,name:`Mark`},name:'requestWelcomeDialog',type:'event',value:''})
    .subscribe((id)=>{
      console.log(`WebChat componentDidMount trigger "requestWelcomeDialog" sent got id ${id}`);
    });
  }
  public render(): React.ReactElement<IWebChatProps> {
    return (
      <div>
        <ReactWebChat directLine={this.directLine} userID={this.props.context.pageContext.user.loginName} username={this.props.context.pageContext.user.displayName}/>
      </div>
    );
  }
}
