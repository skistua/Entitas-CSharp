//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameContext {

    public GameEntity uniqueEntity { get { return GetGroup(GameMatcher.Unique).GetSingleEntity(); } }
    public UniqueComponent unique { get { return uniqueEntity.unique; } }
    public bool hasUnique { get { return uniqueEntity != null; } }

    public GameEntity SetUnique(string newValue) {
        if(hasUnique) {
            throw new Entitas.EntitasException("Could not set Unique!\n" + this + " already has an entity with UniqueComponent!",
                "You should check if the context already has a uniqueEntity before setting it or use context.ReplaceUnique().");
        }
        var entity = CreateEntity();
        entity.AddUnique(newValue);
        return entity;
    }

    public void ReplaceUnique(string newValue) {
        var entity = uniqueEntity;
        if(entity == null) {
            entity = SetUnique(newValue);
        } else {
            entity.ReplaceUnique(newValue);
        }
    }

    public void RemoveUnique() {
        DestroyEntity(uniqueEntity);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public UniqueComponent unique { get { return (UniqueComponent)GetComponent(GameComponentsLookup.Unique); } }
    public bool hasUnique { get { return HasComponent(GameComponentsLookup.Unique); } }

    public void AddUnique(string newValue) {
        var index = GameComponentsLookup.Unique;
        var component = CreateComponent<UniqueComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceUnique(string newValue) {
        var index = GameComponentsLookup.Unique;
        var component = CreateComponent<UniqueComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveUnique() {
        RemoveComponent(GameComponentsLookup.Unique);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherUnique;

    public static Entitas.IMatcher<GameEntity> Unique {
        get {
            if(_matcherUnique == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Unique);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherUnique = matcher;
            }

            return _matcherUnique;
        }
    }
}