namespace System.Runtime.Serialization;

/// <summary>
/// System.Runtime.Serialization.IDataContractSurrogate equivalent.
///
/// .NET did not carry this interface over. DataContractSerializer gained
/// ISerializationSurrogateProvider instead, which is a smaller and differently shaped
/// contract - so there is nothing to alias the old name to, and the type a .NET Framework
/// application implemented simply has no declaration to implement. n2cms's
/// ContentDataContractSurrogate is one, and without this it is the whole N2 library's
/// single remaining build error.
///
/// Declared in the ORIGINAL namespace on purpose, like CallContext next door: ported code
/// says "using System.Runtime.Serialization;" and the point is that the line keeps meaning
/// what it meant.
///
/// **This is a declaration, not a working substitute.** Implementing it makes the
/// application's own surrogate class compile; it does NOT hook the surrogate up to
/// anything, because the serializer that used to consume it does not exist on .NET either.
/// The code that PASSES one to a DataContractSerializer constructor still fails to compile,
/// which is the right place for it to fail - that call is the migration decision, and only
/// the author of the surrogate can make it. A no-op that silently swallowed the surrogate
/// would serialize different XML with nothing to show for it.
///
/// Every member of the original is declared, not only the ones a corpus happens to call.
/// A partial interface is worse than none: the implementing class already overrides all of
/// them, and any member left out becomes CS0539 ("no suitable member to implement") on
/// code that was correct.
/// </summary>
public interface IDataContractSurrogate
{
    Type GetDataContractType(Type type);

    object GetObjectToSerialize(object obj, Type targetType);

    object GetDeserializedObject(object obj, Type targetType);

    void GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes);

    object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType);

    object GetCustomDataToExport(Type clrType, Type dataContractType);

    Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData);

    System.CodeDom.CodeTypeDeclaration ProcessImportedType(
        System.CodeDom.CodeTypeDeclaration typeDeclaration,
        System.CodeDom.CodeCompileUnit compileUnit);
}
