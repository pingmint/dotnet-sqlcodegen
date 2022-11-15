using Pingmint.Yaml;
using Model = Pingmint.CodeGen.Sql.Model.Yaml;

namespace Pingmint.Yaml;

internal sealed class DocumentYaml : IDocument
{
    public Model.Config Model { get; private set; } = new();

    public IMapping? StartMapping() => new ConfigMapping(m => this.Model = m);

    public ISequence? StartSequence() => null;
}

internal sealed class ConfigMapping : Mapping<Model.Config> // passthrough
{
    public ConfigMapping(Action<Model.Config> callback) : base(callback, new()) { } // passthrough

    protected override IMapping? StartMapping(string key) => key switch
    {
        "csharp" => new CSharpMapping(m => this.Model.CSharp = m),
        _ => null,
    };

    protected override ISequence? StartSequence(string key) => key switch
    {
        "databases" => new DatabasesSequence(m => this.Model.Databases = new() { Items = m }),
        _ => null,
    };

    protected override bool Add(string key, string value)
    {
        switch (key)
        {
            case "connection": this.Model.Connection = new() { ConnectionString = value }; return true;
            default: return false;
        }
    }
}

internal sealed class CSharpMapping : Mapping<Model.CSharp>
{
    public CSharpMapping(Action<Model.CSharp> callback) : base(callback, new()) { }

    protected override bool Add(string key, string value)
    {
        switch (key)
        {
            case "namespace": this.Model.Namespace = value; return true;
            case "class": this.Model.ClassName = value; return true;
            default: return false;
        }
    }
}

internal sealed class DatabasesSequence : Sequence<List<Model.DatabasesItem>>
{
    public DatabasesSequence(Action<List<Model.DatabasesItem>> callback) : base(callback, new()) { }

    protected override IMapping? StartMapping() => new DatabaseMapping(m => this.Model.Add(m));
}

internal sealed class DatabaseMapping : Mapping<Model.DatabasesItem>
{
    public DatabaseMapping(Action<Model.DatabasesItem> callback) : base(callback, new()) { }

    protected override ISequence? StartSequence(String key) => key switch
    {
        "procedures" => new ProceduresSequence(m => this.Model.Procedures = m),
        "statements" => new StatementsSequence(m => this.Model.Statements = m),
        _ => null,
    };

    protected override bool Add(string key, string value)
    {
        switch (key)
        {
            case "database": { this.Model.SqlName = value; return true; }
            case "class": { this.Model.ClassName = value; return true; }
            default: return false;
        }
    }
}

// internal sealed class DatabaseItemMapping : Mapping<Model.DatabasesItem>
// {
//     public DatabaseItemMapping(Action<Model.DatabasesItem> callback, String key) : base(callback, new() { SqlName = key }) { }



//     protected override void Pop(List<Model.DatabasesItem> parentModel, Model.DatabasesItem model) => parentModel.Add(this.Model);
// }

internal sealed class ProceduresSequence : Sequence<Model.DatabasesItemProcedures>
{
    public ProceduresSequence(Action<Model.DatabasesItemProcedures> callback) : base(callback, new() { Items = new() }) { }

    protected override IMapping? StartMapping() => new ProcedureMapping(m => this.Model.Items.Add(m));

    protected override bool Add(string value)
    {
        this.Model.Items!.Add(new() { Text = value }); // TODO: remove null-forgiveness
        return true;
    }
}

internal sealed class ProcedureMapping : Mapping<Model.Procedure>
{
    public ProcedureMapping(Action<Model.Procedure> callback) : base(callback, new()) { }

    protected override ISequence? StartSequence(string key) => key switch
    {
        // "parameters" => new ParametersSequence(this.Model.Parameters = new()),
        _ => null,
    };

    protected override bool Add(string key, string value)
    {
        switch (key)
        {
            // case "name": { this.Model.Name = value; return true; }
            // case "text": { this.Model.Text = value; return true; }
            default: return false;
        }
    }
}

internal sealed class StatementsSequence : Sequence<Model.DatabasesItemStatements>
{
    public StatementsSequence(Action<Model.DatabasesItemStatements> callback) : base(callback, new() { Items = new() }) { }

    protected override IMapping? StartMapping() => new StatementMapping(m => this.Model.Items.Add(m));
}

internal sealed class StatementMapping : Mapping<Model.Statement>
{
    public StatementMapping(Action<Model.Statement> callback) : base(callback, new()) { }

    protected override ISequence? StartSequence(string key) => key switch
    {
        "parameters" => new ParametersSequence(m => this.Model.Parameters = new() { Items = m }),
        _ => null,
    };

    protected override bool Add(string key, string value)
    {
        switch (key)
        {
            case "name": { this.Model.Name = value; return true; }
            case "text": { this.Model.Text = value; return true; }
            default: return false;
        }
    }
}

internal sealed class ParametersSequence : Sequence<List<Model.Parameter>>
{
    public ParametersSequence(Action<List<Model.Parameter>> callback) : base(callback, new()) { }

    protected override IMapping? StartMapping() => new ParameterMapping(m => this.Model.Add(m));
}

internal sealed class ParameterMapping : Mapping<Model.Parameter>
{
    public ParameterMapping(Action<Model.Parameter> callback) : base(callback, new()) { }

    protected override bool Add(string key, string value)
    {
        switch (key)
        {
            case "name": { this.Model.Name = value; return true; }
            case "type": { this.Model.Type = value; return true; }
            default: return false;
        }
    }
}
