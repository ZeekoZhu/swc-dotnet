use std::{
    sync::Arc,
};
use std::ffi::{CString};
use anyhow::Error;

use swc_core::{
    base::{
        config::{ErrorFormat, ParseOptions},
        Compiler,
    },
    common::{comments::Comments, FileName},
};

use crate::{get_compiler, util::try_with};
use interoptopus::{ffi_service, ffi_service_ctor, ffi_service_method, ffi_type};

use interoptopus::patterns::option::FFIOption;
use interoptopus::patterns::string::AsciiPointer;
use serde::de::DeserializeOwned;


pub fn deserialize_json<T>(json: &str) -> Result<T, serde_json::Error>
    where
        T: DeserializeOwned,
{
    let mut deserializer = serde_json::Deserializer::from_str(json);
    deserializer.disable_recursion_limit();

    T::deserialize(&mut deserializer)
}

// ----- Parsing -----
#[ffi_type]
#[repr(C)]
pub struct StringRef<'a> {
    pub value: AsciiPointer<'a>,
}


#[ffi_type]
#[repr(C)]
pub struct ParseParams<'a> {
    pub src: AsciiPointer<'a>,
    pub options: AsciiPointer<'a>,
    pub filename: FFIOption<StringRef<'a>>,
}

#[ffi_type(patterns(ffi_error))]
#[repr(C)]
pub enum FFIError {
    Ok = 0,
    Null = 100,
    Panic = 200,
}

impl interoptopus::patterns::result::FFIError for FFIError {
    const SUCCESS: Self = Self::Ok;
    const NULL: Self = Self::Null;
    const PANIC: Self = Self::Panic;
}

impl From<Error> for FFIError {
    fn from(_x: Error) -> Self {
        Self::Panic
    }
}

// Implement Default so we know what the "good" case is.
impl Default for FFIError {
    fn default() -> Self {
        Self::Ok
    }
}

fn parse_with_swc(
    compiler: Arc<Compiler>,
    src: &str,
    options: ParseOptions,
    filename: FileName,
) -> String {
    let fm =
        compiler.cm.new_source_file(filename.clone(), src.to_owned());
    let comments = if options.comments {
        Some(compiler.comments() as &dyn Comments)
    } else {
        None
    };
    let program = try_with(compiler.cm.clone(), false, ErrorFormat::Normal, |handler| {
        compiler.parse_js(
            fm,
            handler,
            options.target,
            options.syntax,
            options.is_module,
            comments,
        )
    }).unwrap();
    serde_json::to_string(&program).unwrap()
}

#[ffi_type(opaque)]
pub struct SwcWrap {
    parse_result: CString,
}

#[ffi_service(error = "FFIError", prefix = "swc_wrap_")]
impl SwcWrap {
    #[ffi_service_ctor]
    pub fn new(options: ParseParams) -> Result<Self, Error> {
        let compiler = get_compiler();
        let parse_opt = options.options.as_str().unwrap();
        let parse_opt = deserialize_json::<ParseOptions>(parse_opt).unwrap();
        let src = options.src.as_str().unwrap();
        let filename = options.filename.into_option()
            .and_then(|x| x.value.as_str()
                .map(|str| FileName::Real(str.into()))
                .map_err(|_| FFIError::Panic)
                .ok())
            .unwrap_or(FileName::Anon);
        let result = parse_with_swc(compiler, src, parse_opt, filename);
        Ok(Self { parse_result: CString::new(result).unwrap() })
    }
    #[ffi_service_method(on_panic = "return_default")]
    pub fn parse(&self) -> AsciiPointer {
        AsciiPointer::from_cstr(&self.parse_result)
    }
}

#[ffi_type(opaque)]
#[derive(Default)]
pub struct Sample {
    pub number: usize,
    name: CString,
}


#[ffi_service(error = "FFIError", prefix = "sample_")]
impl Sample {
    #[ffi_service_ctor]
    pub fn new_with(number: u32) -> Result<Self, Error> {
        Ok(Self {
            number: number as usize,
            name: CString::new(format!("HELLO_{}", number)).unwrap(),
        })
    }

    #[ffi_service_method(on_panic = "return_default")]
    pub fn name(&self) -> AsciiPointer {
        // FIXME: Name can only be called once. second invocation causes crash.
        AsciiPointer::from_cstr(&self.name)
    }
}
