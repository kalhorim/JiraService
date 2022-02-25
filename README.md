# Jira Service

This project is added some extra features to help c# developers that they used Atlassian.SDK.


## Features

- Independent Jira issue model
- Use predefined fields
- Create new own custom field
- Serializable issue model
- Testable issue model


## Installation

Install AtlassianAssistance.JiraService with Nuget

```bash
  Install-Package AtlassianAssistance.JiraService
  or
  dotnet add package AtlassianAssistance.JiraService
```
    


## Usage/Examples

For creating a new model first create a class that inherited from **IssueModel**
```cs
public class ChangeIssue : IssueModel
    {
        
        [CustomField("Case Number", defaultVal: "no={0}")]
        public HyperLinkJField CaseNumber { get; set; }

        [CustomField("Primary Customer")] public CascadeSelectOptionJField PrimaryCustomer { get; set; }

        [CustomField("Customer", "customfield_10808")]
        public InsightJField Customer { get; set; }

        [CustomField("Change Type")] public TextJField ChangeType { get; set; }

        [CustomField("Epic Link")]
        public EpicLinkJField EpicLink { get; set; }
    }
```
If you want your own custom field like **EpicLink** creates a new type that inherited from **JiraCustomFieldBase**, like:

```cs
    public class EpicLinkJField : JiraCustomFieldBase
    {
        private string _value;

        public string Value { get { return _value; } set => _value = value; }

        protected override string[] SetJiraValue { set => _value = string.Join("", value); }

        protected override string GetJiraValue => _value;
    }
```
And now use your type easily in all of your projects it is type-safe and testable.
```cs

// initializing jiraService (GetJiraService has a contract **IJiraService** to make it possible to use mock in test projects.)
var jiraService = new JiraServiceCreator().GetJiraService(setting.HOST, setting.USERNAME, setting.PASSWORD);

// initializing Model
var change = new ChangeIssue();
change.ProjectKey = "test";
change.Type = "test";

//Create
var key = await jiraService.Issue.Create(change);
//Update
await jiraService.Issue.Update(change);
//Delete
await jiraService.Issue.Delete(key);

```
## Acknowledgements

 - [Atlassian.Net SDK](https://bitbucket.org/farmas/atlassian.net-sdk)

## Authors

- [Mohammad Tajik](https://github.com/mtss92)
- [Mohammad Kalhori](https://github.com/kalhorim)


## Contributing

Contributions are always welcome!

## License

[MIT](https://choosealicense.com/licenses/mit/)

## Roadmap

- Create a CLI for creating projects

- Generate models by CLI

- Linq Query


## Feedback

Please add a new issue or add a new discussion if you have any feedback.

