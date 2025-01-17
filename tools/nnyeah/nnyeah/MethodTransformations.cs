using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Microsoft.MaciOS.Nnyeah {
	public class MethodTransformations {
		static List<Transformation>? allTransforms;
		static Dictionary<string, Transformation>? transformTable;

		public Dictionary<string, Transformation> GetTransforms (ModuleContainer modules, Func<List<bool>, CustomAttribute> attrBuilder)
		{
			var moduleToEdit = modules.ModuleToEdit;
			if (transformTable is not null) {
				return transformTable;
			}

			// handy abbreviations
			var intPtrType = moduleToEdit.TypeSystem.IntPtr;
			var uintPtrType = moduleToEdit.TypeSystem.UIntPtr;
			var boolType = moduleToEdit.TypeSystem.Boolean;
			var stringType = moduleToEdit.TypeSystem.String;
			
			// there are two types of transforms here: ones that can be made statically
			// and ones that need more in the way of state and intermediate variables.

			var singleBool = new List<bool> () { true };

			allTransforms = new List<Transformation> (transforms.Length);
			allTransforms.AddRange (transforms);

			// nint
			var mref = new MethodReference ("CompareTo", moduleToEdit.TypeSystem.Int32, intPtrType);
			var parm = new ParameterDefinition (intPtrType);
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			mref.Parameters.Add (parm);
			allTransforms.Add (new Transformation ("System.Int32 System.nint::CompareTo(System.nint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("CompareTo", moduleToEdit.TypeSystem.Int32, intPtrType);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Int32 System.nint::CompareTo(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", boolType, intPtrType);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Boolean System.nint::Equals(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", boolType, intPtrType);
			parm = new ParameterDefinition (intPtrType);
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			mref.Parameters.Add (parm);
			allTransforms.Add (new Transformation ("System.Boolean System.nint::Equals(System.nint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("GetHashCode", moduleToEdit.TypeSystem.Int32, intPtrType);
			allTransforms.Add (new Transformation ("System.Int32 System.nint::GetHashCode ()",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("ToString", stringType, intPtrType);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString()",
				Instruction.Create (OpCodes.Call, mref)));

			var formatProviderTypeRef = moduleToEdit.ImportReference (typeof (IFormatProvider));

			var formatProviderParam = new ParameterDefinition (formatProviderTypeRef);
			mref = new MethodReference ("ToString", stringType, intPtrType);
			mref.Parameters.Add (formatProviderParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString(System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			var stringParam = new ParameterDefinition (stringType);
			mref = new MethodReference ("ToString", stringType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("ToString", stringType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (formatProviderParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nint::ToString(System.String,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", intPtrType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", intPtrType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			var globNumberStylesTypeRef = moduleToEdit.ImportReference (typeof (System.Globalization.NumberStyles));
			var globNumberStylesParam = new ParameterDefinition (globNumberStylesTypeRef);
			mref = new MethodReference ("Parse", intPtrType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String,System.Globalization.NumberStyles)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", intPtrType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::Parse(System.String,System.Globalization.NumberStyles,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			var nintRefParam = new ParameterDefinition (intPtrType);
			nintRefParam.CustomAttributes.Add (attrBuilder (singleBool));
			nintRefParam.IsOut = true;
			mref = new MethodReference ("TryParse", boolType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (nintRefParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nint::TryParse(System.String,System.nint&)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("TryParse", boolType, intPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.Parameters.Add (nintRefParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nint::TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.nint&)",
				Instruction.Create (OpCodes.Call, mref)));

			var decimalTypeReference = new TypeReference ("System", "Decimal", moduleToEdit, moduleToEdit.TypeSystem.CoreLibrary);
			mref = new MethodReference ("op_Implicit", decimalTypeReference, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Int64));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Decimal System.nint::op_Implicit(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_I8),
					Instruction.Create (OpCodes.Call, mref) }));

			mref = new MethodReference ("op_Explicit", moduleToEdit.TypeSystem.Int64, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (decimalTypeReference));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nint System.nint::op_Explicit(System.Decimal)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref),
					Instruction.Create (OpCodes.Conv_I) }));

			var nfloatTypeRef = new TypeReference ("System.Runtime.InteropServices", "NFloat", moduleToEdit, moduleToEdit.TypeSystem.CoreLibrary);
			mref = new MethodReference (".ctor", moduleToEdit.TypeSystem.Void, nfloatTypeRef);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Double));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nfloat System.nint::op_Implicit(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_R8),
					Instruction.Create (OpCodes.Newobj, mref) }));

			var nintVar = new VariableDefinition (moduleToEdit.TypeSystem.Int64);
			var typeCodeRef = new TypeReference ("System", "TypeCode", moduleToEdit, moduleToEdit.TypeSystem.CoreLibrary);
			mref = new MethodReference ("GetTypeCode", typeCodeRef, moduleToEdit.TypeSystem.Int64);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.TypeCode System.nint::GetTypeCode()",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldobj, intPtrType),
					Instruction.Create (OpCodes.Conv_I8),
					Instruction.Create (OpCodes.Stloc, nintVar),
					Instruction.Create (OpCodes.Ldloca, nintVar),
					Instruction.Create (OpCodes.Call, mref)
				}));

			var marshalTypeReference = new TypeReference ("System.Runtime.InteropServices", "Marshal", moduleToEdit, moduleToEdit.TypeSystem.CoreLibrary);
			mref = new MethodReference ("CopyArray", moduleToEdit.TypeSystem.Void, marshalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (intPtrType));
			mref.Parameters.Add (new ParameterDefinition (new ArrayType (intPtrType)));
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Int32));
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Int32));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Void System.nint::CopyArray(System.IntPtr,System.nint[],System.Int32,System.Int32)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref)
				}));

			mref = new MethodReference ("CopyArray", moduleToEdit.TypeSystem.Void, marshalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (new ArrayType (intPtrType)));
			mref.Parameters.Add (new ParameterDefinition (intPtrType));
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Int32));
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Int32));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Void System.nint::CopyArray(System.nint[],System.Int32,System.IntPtr,System.Int32)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref)
				}));

			//"System.nint System.nint::op_Explicit(System.nfloat)"

			// nuint
			mref = new MethodReference ("CompareTo", moduleToEdit.TypeSystem.Int32, uintPtrType);
			mref.Parameters.Add (new ParameterDefinition (uintPtrType));
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			allTransforms.Add (new Transformation ("System.Int32 System.nuint::CompareTo(System.nuint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("CompareTo", moduleToEdit.TypeSystem.Int32, uintPtrType);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Int32 System.nuint::CompareTo(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", boolType, uintPtrType);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Object));
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::Equals(System.Object)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Equals", boolType, uintPtrType);
			parm = new ParameterDefinition (uintPtrType);
			parm.CustomAttributes.Add (attrBuilder (singleBool));
			mref.Parameters.Add (parm);
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::Equals(System.nuint)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("GetHashCode", moduleToEdit.TypeSystem.Int32, uintPtrType);
			allTransforms.Add (new Transformation ("System.Int32 System.nuint::GetHashCode()",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("ToString", stringType, uintPtrType);
			allTransforms.Add (new Transformation ("System.String System.nuint::ToString()",
				Instruction.Create (OpCodes.Call, mref)));

			formatProviderParam = new ParameterDefinition (formatProviderTypeRef);
			mref = new MethodReference ("ToString", stringType, uintPtrType);
			mref.Parameters.Add (formatProviderParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nuint::ToString(System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			stringParam = new ParameterDefinition (stringType);
			mref = new MethodReference ("ToString", stringType, uintPtrType);
			mref.Parameters.Add (stringParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.String System.nuint::ToString(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", uintPtrType, uintPtrType);
			mref.Parameters.Add (stringParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", uintPtrType, uintPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			globNumberStylesTypeRef = moduleToEdit.ImportReference (typeof (System.Globalization.NumberStyles));
			globNumberStylesParam = new ParameterDefinition (globNumberStylesTypeRef);
			mref = new MethodReference ("Parse", uintPtrType, uintPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String,System.Globalization.NumberStyles)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("Parse", uintPtrType, uintPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.MethodReturnType.CustomAttributes.Add (attrBuilder (singleBool));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::Parse(System.String,System.Globalization.NumberStyles,System.IFormatProvider)",
				Instruction.Create (OpCodes.Call, mref)));

			nintRefParam = new ParameterDefinition (uintPtrType);
			nintRefParam.CustomAttributes.Add (attrBuilder (singleBool));
			nintRefParam.IsOut = true;
			mref = new MethodReference ("TryParse", boolType, uintPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (nintRefParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::TryParse(System.String,System.nuint&)",
				Instruction.Create (OpCodes.Call, mref)));

			mref = new MethodReference ("TryParse", boolType, uintPtrType);
			mref.Parameters.Add (stringParam);
			mref.Parameters.Add (globNumberStylesParam);
			mref.Parameters.Add (formatProviderParam);
			mref.Parameters.Add (nintRefParam);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Boolean System.nuint::TryParse(System.String,System.Globalization.NumberStyles,System.IFormatProvider,System.nuint&)",
				Instruction.Create (OpCodes.Call, mref)));

			decimalTypeReference = new TypeReference ("System", "Decimal", moduleToEdit, moduleToEdit.TypeSystem.CoreLibrary);
			mref = new MethodReference ("op_Implicit", decimalTypeReference, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.UInt64));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.Decimal System.nuint::op_Implicit(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_U8),
					Instruction.Create (OpCodes.Call, mref) }));

			mref = new MethodReference ("op_Explicit", moduleToEdit.TypeSystem.UInt64, decimalTypeReference);
			mref.Parameters.Add (new ParameterDefinition (decimalTypeReference));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nuint System.nuint::op_Explicit(System.Decimal)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Call, mref),
					Instruction.Create (OpCodes.Conv_U) }));

			nfloatTypeRef = new TypeReference ("System.Runtime.InteropServices", "NFloat", moduleToEdit, moduleToEdit.TypeSystem.CoreLibrary);
			mref = new MethodReference (".ctor", moduleToEdit.TypeSystem.Void, nfloatTypeRef);
			mref.Parameters.Add (new ParameterDefinition (moduleToEdit.TypeSystem.Double));
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.nfloat System.nuint::op_Implicit(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Conv_R8),
					Instruction.Create (OpCodes.Newobj, mref) }));

			var nuintVar = new VariableDefinition (moduleToEdit.TypeSystem.UInt64);
			mref = new MethodReference ("GetTypeCode", typeCodeRef, moduleToEdit.TypeSystem.UInt64);
			mref = moduleToEdit.ImportReference (mref);
			allTransforms.Add (new Transformation ("System.TypeCode System.nuint::GetTypeCode()",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldobj, uintPtrType),
					Instruction.Create (OpCodes.Conv_U8),
					Instruction.Create (OpCodes.Stloc, nuintVar),
					Instruction.Create (OpCodes.Ldloca, nuintVar),
					Instruction.Create (OpCodes.Call, mref)
				}));

			//"System.nuint System.nuint::op_Explicit(System.nfloat)"

			transformTable = new Dictionary<string, Transformation> ();

			foreach (var xform in allTransforms) {
				transformTable.Add (xform.Operand, xform);
			}
			return transformTable;
		}


		static Transformation [] transforms = new Transformation [] {
			new Transformation (
				"System.nuint System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U)),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.IntPtr)"
				),
			new Transformation(
				"System.IntPtr System.nint::op_Explicit(System.nint)"
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.SByte)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.SByte System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I1)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Byte)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.Byte System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U1)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Char)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.Char System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Int16)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int16 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I2)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.UInt16)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt16 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nint System.nint::op_Implicit(System.Int32)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int32 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I4)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.UInt32)",
				Instruction.Create (OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt32 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_U4)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.Int64)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int64 System.nint::op_Implicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I8)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.UInt64)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation(
				"System.UInt64 System.nint::op_Explicit(System.nint)",
				Instruction.Create (OpCodes.Conv_I8)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.Single)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation (
				"System.Single System.nint::op_Implicit(System.nint)",
				Instruction.Create (OpCodes.Conv_R4)
				),
			new Transformation (
				"System.nint System.nint::op_Explicit(System.Double)",
				Instruction.Create (OpCodes.Conv_I)
				),
			new Transformation (
				"System.Double System.nint::op_Implicit(System.nint)",
				Instruction.Create (OpCodes.Conv_R8)
				),
			new Transformation (
				"System.nint System.nint::op_UnaryPlus(System.nint)"
				),
			new Transformation (
				"System.nint System.nint::op_UnaryNegation(System.nint)",
				Instruction.Create (OpCodes.Neg)
				),
			new Transformation(
				"System.nint System.nint::op_OnesComplement(System.nint)",
				Instruction.Create (OpCodes.Not)
				),
			new Transformation(
				"System.nint System.nint::op_Increment(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Add),
				}
				),
			new Transformation(
				"System.nint System.nint::op_Decrement(System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Sub),
				}
				),
			new Transformation(
				"System.nint System.nint::op_Addition(System.nint,System.nint)",
				Instruction.Create (OpCodes.Add)
				),
			new Transformation(
				"System.nint System.nint::op_Subtraction(System.nint,System.nint)",
				Instruction.Create (OpCodes.Sub)
				),
			new Transformation(
				"System.nint System.nint::op_Multiply(System.nint,System.nint)",
				Instruction.Create (OpCodes.Mul)
				),
			new Transformation(
				"System.nint System.nint::op_Division(System.nint,System.nint)",
				Instruction.Create (OpCodes.Div)
				),
			new Transformation(
				"System.nint System.nint::op_Modulus(System.nint,System.nint)",
				Instruction.Create (OpCodes.Rem)
				),
			new Transformation(
				"System.nint System.nint::op_BitwiseAnd(System.nint,System.nint)",
				Instruction.Create (OpCodes.And)
				),
			new Transformation(
				"System.nint System.nint::op_BitwiseOr(System.nint,System.nint)",
				Instruction.Create (OpCodes.Or)
				),
			new Transformation(
				"System.nint System.nint::op_ExclusiveOr(System.nint,System.nint)",
				Instruction.Create (OpCodes.Xor)
				),
			new Transformation(
				"System.nint System.nint::op_LeftShift(System.nint,System.Int32)",
				Instruction.Create (OpCodes.Shl)
				),
			new Transformation(
				"System.nint System.nint::op_RightShift(System.nint,System.Int32)",
				Instruction.Create (OpCodes.Shr)
				),
			new Transformation(
				"System.Boolean System.nint::op_Equality(System.nint,System.nint)",
				Instruction.Create (OpCodes.Ceq)
				),
			new Transformation(
				"System.Boolean System.nint::op_Inequality(System.nint,System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ceq),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nint::op_LessThan(System.nint,System.nint)",
				Instruction.Create (OpCodes.Clt)
				),
			new Transformation(
				"System.Boolean System.nint::op_GreaterThan(System.nint,System.nint)",
				Instruction.Create (OpCodes.Cgt)
				),
			new Transformation(
				"System.Boolean System.nint::op_LessThanOrEqual(System.nint,System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Cgt),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nint::op_GreaterThanOrEqual(System.nint,System.nint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Clt),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Void System.nint::.ctor(System.nint)"
				),
			new Transformation(
				"System.Void System.nint::.ctor(System.Int32)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Void System.nint::.ctor(System.Int64)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.nint System.nint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Void System.nint::.cctor()"
				),

			new Transformation ("System.Boolean System.nint::System.IConvertible.ToBoolean(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Byte System.nint::System.IConvertible.ToByte(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Char System.nint::System.IConvertible.ToChar(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.DateTime System.nint::System.IConvertible.ToDateTime(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Decimal System.nint::System.IConvertible.ToDecimal(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Double System.nint::System.IConvertible.ToDouble(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Int16 System.nint::System.IConvertible.ToInt16(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Int32 System.nint::System.IConvertible.ToInt32(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Int64 System.nint::System.IConvertible.ToInt64(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.SByte System.nint::System.IConvertible.ToSByte(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Single System.nint::System.IConvertible.ToSingle(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.UInt16 System.nint::System.IConvertible.ToUInt16(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.UInt32 System.nint::System.IConvertible.ToUInt32(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.UInt64 System.nint::System.IConvertible.ToUInt64(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Object System.nint::System.IConvertible.ToType(System.Type,System.IFormatProvider)", Errors.N0001),

			// nuint
			new Transformation(
				"System.Void System.nuint::.ctor(System.UInt32)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Void System.nuint::.ctor(System.UInt64)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Void System.nuint::.ctor(System.nuint)"
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.IntPtr)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.IntPtr System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.UIntPtr)"
				),
			new Transformation(
				"System.UIntPtr System.nuint::op_Explicit(System.nuint)"
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.SByte)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.SByte System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I1)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.Byte)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Byte System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U1)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.Char)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Char System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Int16)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int16 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I2)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.UInt16)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt16 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U2)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Int32)",
				Instruction.Create(OpCodes.Conv_I)
				),
			new Transformation(
				"System.Int32 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_I4)
				),
			new Transformation(
				"System.nuint System.nuint::op_Implicit(System.UInt32)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt32 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U4)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Int64)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Int64 System.nuint::op_Explicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U8)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.UInt64)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.UInt64 System.nuint::op_Implicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_U8)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Single)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Single System.nuint::op_Implicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_R4)
				),
			new Transformation(
				"System.nuint System.nuint::op_Explicit(System.Double)",
				Instruction.Create(OpCodes.Conv_U)
				),
			new Transformation(
				"System.Double System.nuint::op_Implicit(System.nuint)",
				Instruction.Create(OpCodes.Conv_R8)
				),
			new Transformation(
				"System.nuint System.nuint::op_UnaryPlus(System.nuint)"
				),
			new Transformation(
				"System.nuint System.nuint::op_OnesComplement(System.nuint)",
				Instruction.Create(OpCodes.Not)
				),
			new Transformation(
				"System.nuint System.nuint::op_Increment(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Add)
				}
				),
			new Transformation(
				"System.nuint System.nuint::op_Decrement(System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ldc_I4_1),
					Instruction.Create (OpCodes.Sub)
				}
				),
			new Transformation(
				"System.nuint System.nuint::op_Addition(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Add)
				),
			new Transformation(
				"System.nuint System.nuint::op_Subtraction(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Sub)
				),
			new Transformation(
				"System.nuint System.nuint::op_Multiply(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Mul)
				),
			new Transformation(
				"System.nuint System.nuint::op_Division(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Div_Un)
				),
			new Transformation(
				"System.nuint System.nuint::op_Modulus(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Rem_Un)
				),
			new Transformation(
				"System.nuint System.nuint::op_BitwiseAnd(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.And)
				),
			new Transformation(
				"System.nuint System.nuint::op_BitwiseOr(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Or)
				),
			new Transformation(
				"System.nuint System.nuint::op_ExclusiveOr(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Xor)
				),
			new Transformation(
				"System.nuint System.nuint::op_LeftShift(System.nuint,System.Int32)",
				Instruction.Create (OpCodes.Shl)
				),
			new Transformation (
				"System.nuint System.nuint::op_RightShift(System.nuint,System.Int32)",
				Instruction.Create (OpCodes.Shr)
				),
			new Transformation(
				"System.Boolean System.nuint::op_Equality(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Ceq)
				),
			new Transformation(
				"System.Boolean System.nuint::op_Inequality(System.nuint,System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Ceq),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nuint::op_LessThan(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Clt_Un)
				),
			new Transformation(
				"System.Boolean System.nuint::op_GreaterThan(System.nuint,System.nuint)",
				Instruction.Create (OpCodes.Cgt_Un)
				),
			new Transformation(
				"System.Boolean System.nuint::op_LessThanOrEqual(System.nuint,System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Cgt_Un),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation(
				"System.Boolean System.nuint::op_GreaterThanOrEqual(System.nuint,System.nuint)",
				TransformationAction.Replace,
				new List<Instruction> () {
					Instruction.Create (OpCodes.Clt_Un),
					Instruction.Create (OpCodes.Ldc_I4_0),
					Instruction.Create (OpCodes.Ceq)
				}
				),
			new Transformation (
				"System.Void System.nuint::.cctor()"
				),

			new Transformation ("System.Boolean System.nuint::System.IConvertible.ToBoolean(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Byte System.nuint::System.IConvertible.ToByte(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Char System.nuint::System.IConvertible.ToChar(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.DateTime System.nuint::System.IConvertible.ToDateTime(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Decimal System.nuint::System.IConvertible.ToDecimal(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Double System.nuint::System.IConvertible.ToDouble(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Int16 System.nuint::System.IConvertible.ToInt16(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Int32 System.nuint::System.IConvertible.ToInt32(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Int64 System.nuint::System.IConvertible.ToInt64(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.SByte System.nuint::System.IConvertible.ToSByte(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Single System.nuint::System.IConvertible.ToSingle(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.UInt16 System.nuint::System.IConvertible.ToUInt16(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.UInt32 System.nuint::System.IConvertible.ToUInt32(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.UInt64 System.nuint::System.IConvertible.ToUInt64(System.IFormatProvider)", Errors.N0001),
			new Transformation ("System.Object System.nuint::System.IConvertible.ToType(System.Type,System.IFormatProvider)", Errors.N0001),

			// why warnings for this?
			// Because in .NET 6 there is no direct equivalent of this call.
			// In order to support this, we need to inject helper methods that do the actual
			// copying and revector to those helpers.
			// Not doing this right now since a survey of nuget.org shows that these methods
			// are never called in the wild.
			new Transformation ("System.Void System.nuint::CopyArray(System.IntPtr,System.nuint[],System.Int32,System.Int32)",
				Errors.N0002),
			new Transformation ("System.Void System.nuint::CopyArray(System.nuint[],System.Int32,System.IntPtr,System.Int32)",
				Errors.N0002),
		};
	}
}
